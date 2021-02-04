namespace IntegrationTests.Steps
{
    public record Block(Coordinate Coordinate, string Name, string BlockType);
    public record Coordinate(float X, float Y, float Z);

    public record Vector(float X, float Y, float Z);
}