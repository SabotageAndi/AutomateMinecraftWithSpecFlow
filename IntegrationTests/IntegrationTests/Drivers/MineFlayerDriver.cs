using System.Collections.Generic;
using IntegrationTests.Steps;
using RestSharp;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace IntegrationTests.Drivers
{
    public class MineFlayerDriver
    {
        private readonly RestClient _restClient;
        private CultureInfo _englishCultureInfo;

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

        //private Coordinate ConvertPositionToCoordinate(Position response)
        //{
        //    return new Coordinate(float.Parse(response.x, _englishCultureInfo), float.Parse(response.y, _englishCultureInfo), float.Parse(response.z, _englishCultureInfo));
        //}

        public Coordinate MovePlayerToCoordinates(Coordinate goal)
        {
            var request = new RestRequest("position", Method.POST);
            request.AddJsonBody(new
            {
                x = goal.X,
                y = goal.Y,
                z = goal.Z
            }
            );

            var response = _restClient.Post<Position>(request);

            return new Coordinate(response.Data.x, response.Data.y, response.Data.z);
        }

        public void LookAt(Coordinate lookAt)
        {
            var request = new RestRequest("position", Method.POST);
            request.AddJsonBody(new
            {
                x = lookAt.X,
                y = lookAt.Y,
                z = lookAt.Z
            }
            );

            _restClient.Post<Position>(request);
        }

        public void PutBlockInInventory(string blockType, int inventoryPosition)
        {
            var request = new RestRequest("slot", Method.POST);
            request.AddJsonBody(new InventorySlot { position = inventoryPosition, item = blockType });

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

        public List<ItemInInventory> GetChestContent(Coordinate coordinateOfChest)
        {
            var request = new RestRequest("chest", Method.GET);
            request.AddQueryParameter("x", coordinateOfChest.X.ToString("0"));
            request.AddQueryParameter("y", coordinateOfChest.Y.ToString("0"));
            request.AddQueryParameter("z", coordinateOfChest.Z.ToString("0"));

            var response = _restClient.Get<List<ChestItem>>(request);

            return response.Data.Select(i => new ItemInInventory(i.displayName, i.count)).ToList();
        }

        public void PutIntoChest(Coordinate coordinateOfChest, string item, int amount)
        {
            var request = new RestRequest("chest", Method.POST);
            request.AddJsonBody(new
            {
                item = item,
                position = new
                {
                    x = coordinateOfChest.X,
                    y = coordinateOfChest.Y,
                    z = coordinateOfChest.Z
                },
                amount = amount
            });

            var response = _restClient.Post(request);
        }

        private class Position
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

        private class InventorySlot
        {
            public int position { get; set; }
            public string item { get; set; }

            public int amount { get; set; }
        }

        private class ChestItem
        {
            /*
             * [
    {
        "type": 1,
        "count": 2,
        "metadata": 0,
        "nbt": null,
        "name": "stone",
        "displayName": "Stone",
        "stackSize": 64,
        "slot": 0
    }
]
             */
            public string displayName { get; set; }
            public int count { get; set; }
        }
    }
}