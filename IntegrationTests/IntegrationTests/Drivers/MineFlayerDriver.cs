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

            public Position(int x, int y, int z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }

            public int x { get; set; }
            public int y { get; set; }
            public int z { get; set; }
        }
        
    }
}