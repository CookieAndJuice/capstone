import random
import numpy as np
import matplotlib.pyplot as plt
from scipy import ndimage
from scipy.spatial import Delaunay
from scipy.sparse.csgraph import minimum_spanning_tree
from scipy.sparse import csr_matrix

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

        # Add walls around rocks
        walls = np.zeros((self.height, self.width), dtype=int)
        for y in range(self.height):
            for x in range(self.width):
                if self.dungeon[y, x] == 0:
                    if self.count_neighbors(x, y) > 0:
                        walls[y, x] = 1

        # Ensure rocks are fully surrounded by walls
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
        # Label connected components (rocks)
        labeled_array, num_features = ndimage.label(self.dungeon_with_rooms == 2)
        
        # Find the sizes of the labeled regions
        sizes = ndimage.sum(self.dungeon_with_rooms == 2, labeled_array, range(1, num_features + 1))
        
        # Create a mask of the rocks larger than the threshold
        mask = np.zeros_like(self.dungeon_with_rooms)
        for i, size in enumerate(sizes):
            if size >= self.room_threshold:
                mask[labeled_array == i + 1] = 1
        
        # Convert large rocks to rooms (value 3)
        self.dungeon_with_rooms[mask == 1] = 3

        # Create a new array with only rooms and their walls
        self.rooms_and_walls = np.zeros_like(self.dungeon_with_rooms)
        self.rooms_and_walls[self.dungeon_with_rooms == 3] = 2  # Rooms

        # Create walls around rooms
        struct = np.ones((3,3), dtype=bool)  # 3x3 structuring element
        dilated = ndimage.binary_dilation(self.rooms_and_walls == 2, structure=struct)
        self.rooms_and_walls[(dilated) & (self.rooms_and_walls != 2)] = 1  # Walls

        # Calculate room midpoints
        self.calculate_room_midpoints()

    def calculate_room_midpoints(self):
        # Label each room uniquely
        labeled_rooms, num_rooms = ndimage.label(self.rooms_and_walls == 2)
        
        # Calculate the center of mass for each room
        self.room_midpoints = ndimage.center_of_mass(self.rooms_and_walls == 2, labels=labeled_rooms, index=range(1, num_rooms + 1))

    def create_delaunay_triangles(self):
        # Convert room midpoints into a NumPy array
        if len(self.room_midpoints) < 3:
            return None  # Delaunay triangulation requires at least 3 points

        points = np.array(self.room_midpoints)
        delaunay_tri = Delaunay(points)
        return delaunay_tri

    def calculate_mst(self, delaunay_tri):
        points = np.array(self.room_midpoints)
        num_points = len(points)
        
        # Create a distance matrix based on Delaunay triangulation edges
        distance_matrix = np.full((num_points, num_points), np.inf)
        for simplex in delaunay_tri.simplices:
            for i in range(3):
                for j in range(i + 1, 3):
                    p1, p2 = simplex[i], simplex[j]
                    dist = np.linalg.norm(points[p1] - points[p2])
                    distance_matrix[p1, p2] = dist
                    distance_matrix[p2, p1] = dist

        # Calculate MST using Kruskal's algorithm
        mst = minimum_spanning_tree(csr_matrix(distance_matrix)).toarray()
        mst_edges = [(i, j) for i in range(num_points) for j in range(num_points) if mst[i, j] != 0]
        return mst_edges

    def add_random_edges(self, delaunay_tri, mst_edges, probability=0.1):
        points = np.array(self.room_midpoints)
        all_edges = set()

        # Collect all Delaunay edges
        for simplex in delaunay_tri.simplices:
            for i in range(3):
                for j in range(i + 1, 3):
                    edge = tuple(sorted([simplex[i], simplex[j]]))
                    all_edges.add(edge)

        # Remove MST edges from all edges
        mst_edge_set = set(tuple(sorted(edge)) for edge in mst_edges)
        candidate_edges = list(all_edges - mst_edge_set)

        # Randomly select edges based on the given probability
        additional_edges = random.sample(candidate_edges, int(len(candidate_edges) * probability))
        return additional_edges

    def display(self):
        fig, axes = plt.subplots(2, 2, figsize=(9, 9))

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
        delaunay_tri = self.create_delaunay_triangles()
        if delaunay_tri is not None:
            ax2.imshow(self.rooms_and_walls, cmap=cmap_with_rooms, vmin=0, vmax=2)
            ax2.triplot(points[:, 1], points[:, 0], delaunay_tri.simplices, 'c-', lw=1)
            ax2.plot(points[:, 1], points[:, 0], 'o', color='orange', markersize=8, markeredgewidth=2, markeredgecolor='black')
        ax2.set_title('Rooms with Delaunay Triangulation')
        ax2.axis('off')

        # Map 3: Rooms with midpoints and edges in the MST
        ax3 = axes[1, 0]
        mst_edges = self.calculate_mst(delaunay_tri)
        ax3.imshow(self.rooms_and_walls, cmap=cmap_with_rooms, vmin=0, vmax=2)
        for i, j in mst_edges:
            ax3.plot([points[i, 1], points[j, 1]], [points[i, 0], points[j, 0]], 'c-', lw=2)
        ax3.plot(points[:, 1], points[:, 0], 'o', color='orange', markersize=8, markeredgewidth=2, markeredgecolor='black')
        ax3.set_title('Rooms with MST Edges Only')
        ax3.axis('off')

        # Map 4: Rooms with MST and randomly selected additional edges
        ax4 = axes[1, 1]
        additional_edges = self.add_random_edges(delaunay_tri, mst_edges, probability=0.2)
        ax4.imshow(self.rooms_and_walls, cmap=cmap_with_rooms, vmin=0, vmax=2)
        for i, j in mst_edges:
            ax4.plot([points[i, 1], points[j, 1]], [points[i, 0], points[j, 0]], 'c-', lw=2)
        for i, j in additional_edges:
            ax4.plot([points[i, 1], points[j, 1]], [points[i, 0], points[j, 0]], 'c-', lw=1)  # Additional edges in magenta
        ax4.plot(points[:, 1], points[:, 0], 'o', color='orange', markersize=8, markeredgewidth=2, markeredgecolor='black')
        ax4.set_title('Rooms with MST and Additional Random Edges')
        ax4.axis('off')

        plt.tight_layout()
        plt.show()

# Usage
generator = DungeonGenerator(width=100, height=100, rock_percentage=0.35, room_threshold=15)
generator.generate()
generator.display()
