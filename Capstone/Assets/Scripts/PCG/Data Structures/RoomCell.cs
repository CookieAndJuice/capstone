namespace PCG.Data_Structures
{
    public class RoomCell : Cell
    {  
        public int RoomNumber { get; set; }
        public bool IsCenter { get; set; }
        
        public RoomCell(int x,
            int y,
            CellType type = CellType.Room,
            int roomNumber = 0,
            bool isCenter = false) : base(x, y, type)
        {
            RoomNumber = roomNumber;
            IsCenter = isCenter;
        }
    }
}