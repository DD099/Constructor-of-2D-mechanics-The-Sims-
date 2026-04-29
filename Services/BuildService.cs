using Microsoft.Extensions.Options;
using SimsConstructor.Models.Building;
using SimsConstructor.Options;

namespace SimsConstructor.Services;

public sealed class BuildService
{
    private readonly float _roomWidthUnits;
    private readonly float _roomHeightUnits;

    private readonly List<WallSegment> _walls = [];
    private readonly List<RoomArea> _rooms = [];

    private bool _roomsDirty = true;

    public string? StatusMessage { get; private set; }

    public float GridStepMeters
    {
        get => _gridStepMeters;
        set
        {
            if (value <= 0f)
                return;
            _gridStepMeters = value;
            _roomsDirty = true;
        }
    }

    private float _gridStepMeters = 0.25f;

    public IReadOnlyList<WallSegment> Walls => _walls;

    public IReadOnlyList<RoomArea> Rooms
    {
        get
        {
            RecomputeRoomsIfNeeded();
            return _rooms;
        }
    }

    public BuildService(IOptions<RoomSettings> roomOptions)
    {
        var r = roomOptions.Value;
        _roomWidthUnits = r.WidthMeters > 0f ? r.WidthMeters : 6f;
        _roomHeightUnits = r.HeightMeters > 0f ? r.HeightMeters : 4f;
    }

    public void SetStatus(string? message) => StatusMessage = message;

    public float RoomWidthUnits => _roomWidthUnits;

    public float RoomHeightUnits => _roomHeightUnits;

    public RoomArea? GetRoomAt(float centerX, float centerY)
    {
        foreach (var room in Rooms)
        {
            if (centerX >= room.X
                && centerX <= room.X + room.Width
                && centerY >= room.Y
                && centerY <= room.Y + room.Height)
                return room;
        }

        return null;
    }

    public bool TryAddWallSegment(
        float x1,
        float y1,
        float x2,
        float y2,
        out string? error)
    {
        StatusMessage = null;
        error = null;

        if (GridStepMeters <= 0f)
        {
            error = "Invalid grid step.";
            return false;
        }

        var x1s = Snap(x1, GridStepMeters);
        var x2s = Snap(x2, GridStepMeters);
        var y1s = Snap(y1, GridStepMeters);
        var y2s = Snap(y2, GridStepMeters);

        var eps = 1e-4f;

        var isVertical = MathF.Abs(x1s - x2s) <= eps;
        var isHorizontal = MathF.Abs(y1s - y2s) <= eps;

        if (isVertical == isHorizontal)
        {
            error = "Walls must be axis-aligned.";
            return false;
        }

        if (isVertical)
        {
            var x = x1s;
            var yMin = MathF.Min(y1s, y2s);
            var yMax = MathF.Max(y1s, y2s);

            if (!InBounds(x, 0f, _roomWidthUnits) || !InBounds(yMin, 0f, _roomHeightUnits) || !InBounds(yMax, 0f, _roomHeightUnits))
            {
                error = "Wall is out of bounds.";
                return false;
            }

            if (yMax - yMin < GridStepMeters - eps)
            {
                error = "Wall is too short.";
                return false;
            }

            var mergedMin = yMin;
            var mergedMax = yMax;
            for (var i = _walls.Count - 1; i >= 0; i--)
            {
                var w = _walls[i];
                if (w.Orientation != WallOrientation.Vertical)
                    continue;
                if (MathF.Abs(w.X1 - x) > eps)
                    continue;

                if (w.MinY <= mergedMax + eps && w.MaxY >= mergedMin - eps)
                {
                    mergedMin = MathF.Min(mergedMin, w.MinY);
                    mergedMax = MathF.Max(mergedMax, w.MaxY);
                    _walls.RemoveAt(i);
                }
            }

            _walls.Add(new WallSegment(WallOrientation.Vertical, x, mergedMin, x, mergedMax));
            _roomsDirty = true;
            return true;
        }

        // Horizontal
        {
            var y = y1s;
            var xMin = MathF.Min(x1s, x2s);
            var xMax = MathF.Max(x1s, x2s);

            if (!InBounds(y, 0f, _roomHeightUnits) || !InBounds(xMin, 0f, _roomWidthUnits) || !InBounds(xMax, 0f, _roomWidthUnits))
            {
                error = "Wall is out of bounds.";
                return false;
            }

            if (xMax - xMin < GridStepMeters - eps)
            {
                error = "Wall is too short.";
                return false;
            }

            var mergedMin = xMin;
            var mergedMax = xMax;
            for (var i = _walls.Count - 1; i >= 0; i--)
            {
                var w = _walls[i];
                if (w.Orientation != WallOrientation.Horizontal)
                    continue;
                if (MathF.Abs(w.Y1 - y) > eps)
                    continue;

                if (w.MinX <= mergedMax + eps && w.MaxX >= mergedMin - eps)
                {
                    mergedMin = MathF.Min(mergedMin, w.MinX);
                    mergedMax = MathF.Max(mergedMax, w.MaxX);
                    _walls.RemoveAt(i);
                }
            }

            _walls.Add(new WallSegment(WallOrientation.Horizontal, mergedMin, y, mergedMax, y));
            _roomsDirty = true;
            return true;
        }
    }

    public bool TryEraseWallSegment(
        float x1,
        float y1,
        float x2,
        float y2,
        out string? error)
    {
        StatusMessage = null;
        error = null;

        if (GridStepMeters <= 0f)
        {
            error = "Invalid grid step.";
            return false;
        }

        var x1s = Snap(x1, GridStepMeters);
        var x2s = Snap(x2, GridStepMeters);
        var y1s = Snap(y1, GridStepMeters);
        var y2s = Snap(y2, GridStepMeters);

        var eps = 1e-4f;

        var isVertical = MathF.Abs(x1s - x2s) <= eps;
        var isHorizontal = MathF.Abs(y1s - y2s) <= eps;

        if (isVertical == isHorizontal)
        {
            error = "Walls must be axis-aligned.";
            return false;
        }

        var changed = false;
        if (isVertical)
        {
            var x = x1s;
            var yMin = MathF.Min(y1s, y2s);
            var yMax = MathF.Max(y1s, y2s);

            if (!InBounds(x, 0f, _roomWidthUnits) || !InBounds(yMin, 0f, _roomHeightUnits) || !InBounds(yMax, 0f, _roomHeightUnits))
            {
                error = "Erase segment is out of bounds.";
                return false;
            }

            for (var i = _walls.Count - 1; i >= 0; i--)
            {
                var w = _walls[i];
                if (w.Orientation != WallOrientation.Vertical)
                    continue;
                if (MathF.Abs(w.X1 - x) > eps)
                    continue;

                // no overlap
                if (yMax <= w.MinY + eps || yMin >= w.MaxY - eps)
                    continue;

                changed = true;
                _walls.RemoveAt(i);

                // left remainder
                var leftEnd = MathF.Min(yMin, w.MaxY);
                if (w.MinY < leftEnd - eps)
                    _walls.Add(new WallSegment(WallOrientation.Vertical, x, w.MinY, x, leftEnd));

                // right remainder
                var rightStart = MathF.Max(yMax, w.MinY);
                if (rightStart < w.MaxY - eps)
                    _walls.Add(new WallSegment(WallOrientation.Vertical, x, rightStart, x, w.MaxY));
            }
        }
        else
        {
            var y = y1s;
            var xMin = MathF.Min(x1s, x2s);
            var xMax = MathF.Max(x1s, x2s);

            if (!InBounds(y, 0f, _roomHeightUnits) || !InBounds(xMin, 0f, _roomWidthUnits) || !InBounds(xMax, 0f, _roomWidthUnits))
            {
                error = "Erase segment is out of bounds.";
                return false;
            }

            for (var i = _walls.Count - 1; i >= 0; i--)
            {
                var w = _walls[i];
                if (w.Orientation != WallOrientation.Horizontal)
                    continue;
                if (MathF.Abs(w.Y1 - y) > eps)
                    continue;

                if (xMax <= w.MinX + eps || xMin >= w.MaxX - eps)
                    continue;

                changed = true;
                _walls.RemoveAt(i);

                var leftEnd = MathF.Min(xMin, w.MaxX);
                if (w.MinX < leftEnd - eps)
                    _walls.Add(new WallSegment(WallOrientation.Horizontal, w.MinX, y, leftEnd, y));

                var rightStart = MathF.Max(xMax, w.MinX);
                if (rightStart < w.MaxX - eps)
                    _walls.Add(new WallSegment(WallOrientation.Horizontal, rightStart, y, w.MaxX, y));
            }
        }

        if (changed)
        {
            _roomsDirty = true;
            return true;
        }

        return true;
    }

    private void RecomputeRoomsIfNeeded()
    {
        if (!_roomsDirty)
            return;

        _roomsDirty = false;
        _rooms.Clear();

        if (_walls.Count == 0)
            return;

        var step = GridStepMeters;
        var eps = 1e-4f;

        var cols = (int)MathF.Round(_roomWidthUnits / step);
        var rows = (int)MathF.Round(_roomHeightUnits / step);
        if (cols <= 0 || rows <= 0)
            return;

        var hWalls = new bool[rows + 1, cols]; // [yLine][xCell]
        var vWalls = new bool[cols + 1, rows]; // [xLine][yCell]

        foreach (var w in _walls)
        {
            if (w.Orientation == WallOrientation.Horizontal)
            {
                var yLine = (int)MathF.Round(w.Y1 / step);
                var xStart = (int)MathF.Round(w.MinX / step);
                var xEnd = (int)MathF.Round(w.MaxX / step);
                if (yLine < 0 || yLine > rows)
                    continue;
                if (xStart < 0)
                    xStart = 0;
                if (xEnd > cols)
                    xEnd = cols;

                for (var x = xStart; x < xEnd; x++)
                    hWalls[yLine, x] = true;
            }
            else
            {
                var xLine = (int)MathF.Round(w.X1 / step);
                var yStart = (int)MathF.Round(w.MinY / step);
                var yEnd = (int)MathF.Round(w.MaxY / step);
                if (xLine < 0 || xLine > cols)
                    continue;
                if (yStart < 0)
                    yStart = 0;
                if (yEnd > rows)
                    yEnd = rows;

                for (var y = yStart; y < yEnd; y++)
                    vWalls[xLine, y] = true;
            }
        }

        // Mark "outside" region reachable from canvas boundary where outer edges are open.
        var outside = new bool[cols, rows];
        var q = new Queue<(int X, int Y)>();

        void EnqueueIfOpen(int xCell, int yCell)
        {
            if (outside[xCell, yCell])
                return;
            outside[xCell, yCell] = true;
            q.Enqueue((xCell, yCell));
        }

        // bottom / top boundary openings
        for (var x = 0; x < cols; x++)
        {
            if (!hWalls[0, x])
                EnqueueIfOpen(x, 0);
            if (!hWalls[rows, x])
                EnqueueIfOpen(x, rows - 1);
        }

        // left / right boundary openings
        for (var y = 0; y < rows; y++)
        {
            if (!vWalls[0, y])
                EnqueueIfOpen(0, y);
            if (!vWalls[cols, y])
                EnqueueIfOpen(cols - 1, y);
        }

        while (q.Count > 0)
        {
            var (x, y) = q.Dequeue();

            // right: cross vertical boundary at x = x+1
            if (x + 1 < cols && !vWalls[x + 1, y] && !outside[x + 1, y])
                EnqueueIfOpen(x + 1, y);

            // left: cross vertical boundary at x = x
            if (x - 1 >= 0 && !vWalls[x, y] && !outside[x - 1, y])
                EnqueueIfOpen(x - 1, y);

            // up: cross horizontal boundary at y = y+1
            if (y + 1 < rows && !hWalls[y + 1, x] && !outside[x, y + 1])
                EnqueueIfOpen(x, y + 1);

            // down: cross horizontal boundary at y = y
            if (y - 1 >= 0 && !hWalls[y, x] && !outside[x, y - 1])
                EnqueueIfOpen(x, y - 1);
        }

        var visited = (bool[,])outside.Clone();
        var roomIndex = 0;

        for (var sx = 0; sx < cols; sx++)
        {
            for (var sy = 0; sy < rows; sy++)
            {
                if (visited[sx, sy])
                    continue;

                var set = new HashSet<int>();
                var cells = new List<(int X, int Y)>();

                var minX = sx;
                var maxX = sx;
                var minY = sy;
                var maxY = sy;

                var regionQueue = new Queue<(int X, int Y)>();
                regionQueue.Enqueue((sx, sy));
                visited[sx, sy] = true;

                while (regionQueue.Count > 0)
                {
                    var (x, y) = regionQueue.Dequeue();
                    var key = x + y * cols;
                    if (!set.Add(key))
                        continue;
                    cells.Add((x, y));
                    minX = Math.Min(minX, x);
                    maxX = Math.Max(maxX, x);
                    minY = Math.Min(minY, y);
                    maxY = Math.Max(maxY, y);

                    // adjacency rules are identical to outside BFS: do not cross walls.
                    if (x + 1 < cols && !vWalls[x + 1, y] && !visited[x + 1, y])
                    {
                        visited[x + 1, y] = true;
                        regionQueue.Enqueue((x + 1, y));
                    }
                    if (x - 1 >= 0 && !vWalls[x, y] && !visited[x - 1, y])
                    {
                        visited[x - 1, y] = true;
                        regionQueue.Enqueue((x - 1, y));
                    }
                    if (y + 1 < rows && !hWalls[y + 1, x] && !visited[x, y + 1])
                    {
                        visited[x, y + 1] = true;
                        regionQueue.Enqueue((x, y + 1));
                    }
                    if (y - 1 >= 0 && !hWalls[y, x] && !visited[x, y - 1])
                    {
                        visited[x, y - 1] = true;
                        regionQueue.Enqueue((x, y - 1));
                    }
                }

                var widthCells = maxX - minX + 1;
                var heightCells = maxY - minY + 1;
                var expectedCellCount = widthCells * heightCells;

                if (cells.Count != expectedCellCount)
                    continue;

                // Verify region fully fills the bounding rectangle.
                var isRectangle = true;
                for (var x = minX; x <= maxX && isRectangle; x++)
                {
                    for (var y = minY; y <= maxY; y++)
                    {
                        if (!set.Contains(x + y * cols))
                        {
                            isRectangle = false;
                            break;
                        }
                    }
                }

                if (!isRectangle)
                    continue;

                var roomX = minX * step;
                var roomY = minY * step;
                var roomW = widthCells * step;
                var roomH = heightCells * step;

                if (roomW < step - eps || roomH < step - eps)
                    continue;

                _rooms.Add(new RoomArea(roomIndex, roomX, roomY, roomW, roomH));
                roomIndex++;
            }
        }
    }

    private static float Snap(float value, float step)
    {
        if (step <= 0f)
            return value;

        return MathF.Round(value / step) * step;
    }

    private static bool InBounds(float value, float min, float max) =>
        value >= min && value <= max + 1e-4f;
}

