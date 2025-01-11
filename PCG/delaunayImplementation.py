import random
import numpy as np
import matplotlib.pyplot as plt
from scipy import ndimage

class DungeonGenerator:
    def __init__(self, width, height, rock_percentage=0.35, generations=3, birth_limit=4, death_limit=3, room_threshold=20):
        self.width = width
        self.height = height
        self.rock_percentage = rock_percentage
        self.generations = generations
        self.birth_limit = birth_limit
        self.death_limit = death_limit
        self.room_threshold = room_threshold
        self.dungeon = np.zeros((height, width), dtype=int)
        self.dungeon_with_rooms = None
        self.rooms_and_walls = None
        self.room_midpoints = []

    def initialize(self):
        total_cells = self.width * self.height
        rock_cells = int(total_cells * self.rock_percentage)
        rock_indices = random.sample(range(total_cells), rock_cells)
        self.dungeon.flat[rock_indices] = 1

    def count_neighbors(self, x, y):
        count = 0
        for i in range(-1, 2):
            for j in range(-1, 2):
                if i == 0 and j == 0:
                    continue
                ni, nj = y + i, x + j
                if 0 <= ni < self.height and 0 <= nj < self.width:
                    count += (self.dungeon[ni, nj] == 1)
        return count

    def generate(self):
        self.initialize()
        for _ in range(self.generations):
            new_dungeon = np.copy(self.dungeon)
            for y in range(self.height):
                for x in range(self.width):
                    neighbors = self.count_neighbors(x, y)
                    if self.dungeon[y, x] == 1:
                        if neighbors < self.death_limit:
                            new_dungeon[y, x] = 0
                    else:
                        if neighbors > self.birth_limit:
                            new_dungeon[y, x] = 1
            self.dungeon = new_dungeon

        walls = np.zeros((self.height, self.width), dtype=int)
        for y in range(self.height):
            for x in range(self.width):
                if self.dungeon[y, x] == 0:
                    if self.count_neighbors(x, y) > 0:
                        walls[y, x] = 1

        for y in range(self.height):
            for x in range(self.width):
                if self.dungeon[y, x] == 1:
                    for i in range(-1, 2):
                        for j in range(-1, 2):
                            ni, nj = y + i, x + j
                            if 0 <= ni < self.height and 0 <= nj < self.width and self.dungeon[ni, nj] == 0:
                                walls[ni, nj] = 1

        self.dungeon = self.dungeon * 2 + walls
        self.dungeon_with_rooms = np.copy(self.dungeon)
        self.create_rooms()

    def create_rooms(self):
        labeled_array, num_features = ndimage.label(self.dungeon_with_rooms == 2)
        sizes = ndimage.sum(self.dungeon_with_rooms == 2, labeled_array, range(1, num_features + 1))
        
        mask = np.zeros_like(self.dungeon_with_rooms)
        for i, size in enumerate(sizes):
            if size >= self.room_threshold:
                mask[labeled_array == i + 1] = 1
        
        self.dungeon_with_rooms[mask == 1] = 3
        self.rooms_and_walls = np.zeros_like(self.dungeon_with_rooms)
        self.rooms_and_walls[self.dungeon_with_rooms == 3] = 2

        struct = np.ones((3,3), dtype=bool)
        dilated = ndimage.binary_dilation(self.rooms_and_walls == 2, structure=struct)
        self.rooms_and_walls[(dilated) & (self.rooms_and_walls != 2)] = 1

        self.calculate_room_midpoints()

    def calculate_room_midpoints(self):
        labeled_rooms, num_rooms = ndimage.label(self.rooms_and_walls == 2)
        self.room_midpoints = ndimage.center_of_mass(self.rooms_and_walls == 2, labels=labeled_rooms, index=range(1, num_rooms + 1))

    def circumcircle(self, p1, p2, p3):
        """Calculate the center and radius of the circumcircle of three points"""
        d = 2 * (p1[0] * (p2[1] - p3[1]) + p2[0] * (p3[1] - p1[1]) + p3[0] * (p1[1] - p2[1]))
        if d == 0:
            return None, None
        
        ux = ((p1[0] * p1[0] + p1[1] * p1[1]) * (p2[1] - p3[1]) + 
              (p2[0] * p2[0] + p2[1] * p2[1]) * (p3[1] - p1[1]) + 
              (p3[0] * p3[0] + p3[1] * p3[1]) * (p1[1] - p2[1])) / d
        
        uy = ((p1[0] * p1[0] + p1[1] * p1[1]) * (p3[0] - p2[0]) + 
              (p2[0] * p2[0] + p2[1] * p2[1]) * (p1[0] - p3[0]) + 
              (p3[0] * p3[0] + p3[1] * p3[1]) * (p2[0] - p1[0])) / d
        
        center = (ux, uy)
        radius = np.sqrt((p1[0] - center[0])**2 + (p1[1] - center[1])**2)
        return center, radius

    def point_in_circumcircle(self, p, triangle):
        """Check if a point lies inside the circumcircle of a triangle"""
        center, radius = self.circumcircle(triangle[0], triangle[1], triangle[2])
        if center is None:
            return False
        return np.sqrt((p[0] - center[0])**2 + (p[1] - center[1])**2) < radius

    def create_delaunay_triangles(self):
        if len(self.room_midpoints) < 3:
            return None

        points = np.array(self.room_midpoints)
        
        # Start with a super triangle that contains all points
        max_x = np.max(points[:, 1]) + 1
        min_x = np.min(points[:, 1]) - 1
        max_y = np.max(points[:, 0]) + 1
        min_y = np.min(points[:, 0]) - 1
        dx = max_x - min_x
        dy = max_y - min_y
        
        # Define a super triangle sufficiently large to encompass all points
        super_triangle = np.array([
            [min_y - dy, min_x - dx],
            [min_y - dy, max_x + dx],
            [max_y + dy * 2, (min_x + max_x) / 2]
        ])
        
        triangles = [super_triangle]

        for point in points:
            edges_buffer = []

            # Find all triangles where the point lies within their circumcircle
            triangles_to_remove = []
            for i, triangle in enumerate(triangles):
                if self.point_in_circumcircle(point, triangle):
                    # Collect edges of the triangles to be removed
                    edges_buffer.extend([
                        (tuple(triangle[0]), tuple(triangle[1])),
                        (tuple(triangle[1]), tuple(triangle[2])),
                        (tuple(triangle[2]), tuple(triangle[0]))
                    ])
                    triangles_to_remove.append(i)

            # Remove triangles whose circumcircles contain the point
            triangles = [t for i, t in enumerate(triangles) if i not in triangles_to_remove]

            # Deduplicate edges by removing pairs that appear twice
            unique_edges = []
            for edge in edges_buffer:
                reverse_edge = (edge[1], edge[0])
                if edge in unique_edges:
                    unique_edges.remove(edge)
                elif reverse_edge in unique_edges:
                    unique_edges.remove(reverse_edge)
                else:
                    unique_edges.append(edge)

            # Form new triangles with the point and each unique edge
            for edge in unique_edges:
                triangles.append(np.array([edge[0], edge[1], point]))

        # Remove triangles connected to super triangle vertices
        super_points = set([tuple(p) for p in super_triangle])
        final_triangles = [triangle for triangle in triangles if not any(tuple(p) in super_points for p in triangle)]
        
        return np.array(final_triangles)


    def calculate_mst(self, triangulation):
        if triangulation is None or len(triangulation) == 0:
            return []

        points = np.array(self.room_midpoints)
        num_points = len(points)
        
        # Create edges from triangulation with weights
        edges = set()
        for triangle in triangulation:
            for i in range(3):
                p1 = tuple(triangle[i])
                p2 = tuple(triangle[(i + 1) % 3])
                # Find indices of these points in the original points array
                idx1 = next(i for i, p in enumerate(points) if tuple(p) == p1)
                idx2 = next(i for i, p in enumerate(points) if tuple(p) == p2)
                weight = np.sqrt((p1[0] - p2[0])**2 + (p1[1] - p2[1])**2)
                edges.add((idx1, idx2, weight))

        # Kruskal's algorithm
        edges = sorted(edges, key=lambda x: x[2])  # Sort edges by weight
        parent = list(range(num_points))
        rank = [0] * num_points
        
        def find(x):
            if parent[x] != x:
                parent[x] = find(parent[x])
            return parent[x]
        
        def union(x, y):
            px, py = find(x), find(y)
            if px == py:
                return
            if rank[px] < rank[py]:
                parent[px] = py
            elif rank[px] > rank[py]:
                parent[py] = px
            else:
                parent[py] = px
                rank[px] += 1
        
        mst_edges = []
        for u, v, w in edges:
            if find(u) != find(v):
                union(u, v)
                mst_edges.append((u, v))
        
        return mst_edges

    def add_random_edges(self, triangulation, mst_edges, probability=0.1):
        if triangulation is None:
            return []

        # Create a mapping from point coordinates to indices
        point_to_index = {tuple(p): i for i, p in enumerate(self.room_midpoints)}
        
        all_edges = set()
        for triangle in triangulation:
            # Get indices for each pair of points in the triangle
            for i in range(3):
                p1 = tuple(triangle[i])
                p2 = tuple(triangle[(i + 1) % 3])
                if p1 in point_to_index and p2 in point_to_index:
                    edge = tuple(sorted([point_to_index[p1], point_to_index[p2]]))
                    all_edges.add(edge)

        mst_edge_set = set(tuple(sorted(edge)) for edge in mst_edges)
        candidate_edges = list(all_edges - mst_edge_set)
        
        # Ensure we don't try to sample more edges than available
        num_edges = min(int(len(candidate_edges) * probability), len(candidate_edges))
        return random.sample(candidate_edges, num_edges) if num_edges > 0 else []

    def display(self):
        fig, axes = plt.subplots(2, 2, figsize=(8, 8))

        colors_with_rooms = ['white', 'red', 'blue']
        cmap_with_rooms = plt.cm.colors.ListedColormap(colors_with_rooms)

        # Map 1: Rooms with midpoints only
        ax1 = axes[0, 0]
        ax1.imshow(self.rooms_and_walls, cmap=cmap_with_rooms, vmin=0, vmax=2)
        points = np.array(self.room_midpoints)
        ax1.plot(points[:, 1], points[:, 0], 'o', color='orange', markersize=8, markeredgewidth=2, markeredgecolor='black')
        ax1.set_title('Rooms with Midpoints Only')
        ax1.axis('off')

        # Map 2: Rooms with midpoints and Delaunay triangulation
        ax2 = axes[0, 1]
        triangulation = self.create_delaunay_triangles()
        if triangulation is not None:
            ax2.imshow(self.rooms_and_walls, cmap=cmap_with_rooms, vmin=0, vmax=2)
            for triangle in triangulation:
                ax2.plot([triangle[0][1], triangle[1][1], triangle[2][1], triangle[0][1]], 
                        [triangle[0][0], triangle[1][0], triangle[2][0], triangle[0][0]], 'c-', lw=1)
            ax2.plot(points[:, 1], points[:, 0], 'o', color='orange', markersize=8, markeredgewidth=2, markeredgecolor='black')
        ax2.set_title('Rooms with Delaunay Triangulation')
        ax2.axis('off')

        # Map 3: Rooms with midpoints and edges in the MST
        ax3 = axes[1, 0]
        mst_edges = self.calculate_mst(triangulation)
        ax3.imshow(self.rooms_and_walls, cmap=cmap_with_rooms, vmin=0, vmax=2)
        for i, j in mst_edges:
            ax3.plot([points[i, 1], points[j, 1]], [points[i, 0], points[j, 0]], 'c-', lw=2)
        ax3.plot(points[:, 1], points[:, 0], 'o', color='orange', markersize=8, markeredgewidth=2, markeredgecolor='black')
        ax3.set_title('Rooms with MST Edges Only')
        ax3.axis('off')

        # Map 4: Rooms with MST and randomly selected additional edges
        ax4 = axes[1, 1]
        additional_edges = self.add_random_edges(triangulation, mst_edges, probability=0.2)
        ax4.imshow(self.rooms_and_walls, cmap=cmap_with_rooms, vmin=0, vmax=2)
        for i, j in mst_edges:
            ax4.plot([points[i, 1], points[j, 1]], [points[i, 0], points[j, 0]], 'c-', lw=2)
        for i, j in additional_edges:
            ax4.plot([points[i, 1], points[j, 1]], [points[i, 0], points[j, 0]], 'c-', lw=1)
        ax4.plot(points[:, 1], points[:, 0], 'o', color='orange', markersize=8, markeredgewidth=2, markeredgecolor='black')
        ax4.set_title('Rooms with MST and Additional Random Edges')
        ax4.axis('off')

        plt.tight_layout()
        plt.show()

# Usage
generator = DungeonGenerator(width=100, height=100, rock_percentage=0.35, room_threshold=15)
generator.generate()
generator.display()