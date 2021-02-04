using System.Threading;
using IntegrationTests.Steps;
using RestSharp;

namespace IntegrationTests.Drivers
{
    public class MineFlayerDriver
    {
        private RestClient _restClient;

        public MineFlayerDriver()
        {
            _restClient = new RestClient("http://localhost:3000");
        }

        public Coordinate GetPlayerCoordinate()
        {
            var request = new RestRequest("position", Method.GET);

            var response = _restClient.Get<Position>(request);

            return new Coordinate(response.Data.x, response.Data.y, response.Data.z);
        }

        public Coordinate MovePlayerToCoordinates(Coordinate goal)
        {
            var request = new RestRequest("position", Method.POST);
            request.AddJsonBody(new Position(goal.X, goal.Y, goal.Z));

            var response = _restClient.Post<Position>(request);

            return new Coordinate(response.Data.x, response.Data.y, response.Data.z);
        }

        public void LookAt(Coordinate lookAt)
        {
            var request = new RestRequest("position", Method.POST);
            request.AddJsonBody(new Position(lookAt.X, lookAt.Y, lookAt.Z));

            _restClient.Post<Position>(request);
        }

        class Position
        {
            public Position()
            {
            }

            public Position(float x, float y, float z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }

            public float x { get; set; }
            public float y { get; set; }
            public float z { get; set; }
        }

        class InventorySlot
        {
            public int position { get; set; }
            public string item { get; set; }
        }

        public void PutBlockInInventory(Block block, int inventoryPosition)
        {

            var request = new RestRequest("slot", Method.POST);
            request.AddJsonBody(new InventorySlot() { position = inventoryPosition, item = block.BlockType });

            _restClient.Post(request);

            Thread.Sleep(500);
        }

        public void PlaceBlock(Block block, Coordinate coordinateBlock, Vector placementVector)
        {
            var request = new RestRequest("placeBlock", Method.POST);
            request.AddJsonBody(new
            {
                item = block.BlockType,
                position = new
                {
                    x = coordinateBlock.X,
                    y = coordinateBlock.Y,
                    z = coordinateBlock.Z
                },
                placementVector = new
                {
                    x = placementVector.X,
                    y = placementVector.Y,
                    z = placementVector.Z
                }

            });


            _restClient.Post(request);
        }
    }
}