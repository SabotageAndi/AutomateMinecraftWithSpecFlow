namespace IntegrationTests.Steps
{
    public record Block(Coordinate Coordinate, string Name, string BlockType);
    public record Coordinate(int X, int Y, int Z);

    public record Vector(int X, int Y, int Z);
}