using System;
using System.Threading;
using IntegrationTests.Drivers;
using TechTalk.SpecFlow;

namespace IntegrationTests.Steps
{
    [Binding]
    class PlayerSteps
    {
        private readonly WorldContext _worldContext;
        private readonly MineFlayerDriver _mineFlayerDriver;

        public PlayerSteps(WorldContext worldContext, MineFlayerDriver mineFlayerDriver)
        {
            _worldContext = worldContext;
            _mineFlayerDriver = mineFlayerDriver;
        }

        [Given(@"there is the '(.*)' at X:(.*),Z:(.*)")]
        public void GivenThereIsTheAtXZ(string blockName, int x, int z)
        {
            var coordinateBlock = new Coordinate(x, _worldContext.YLevel, z);
            var neighbourBlock = coordinateBlock with { Y = coordinateBlock.Y - 1 };

            var placementVector = new Vector(0, 1, 0);

            var block = new Block(coordinateBlock, blockName, GetBlockType(blockName));
            _worldContext.Blocks[blockName] = block;


            var coordinateForPlayer = coordinateBlock with { X = coordinateBlock.X - 1 };




            // go to coord one coord near block
            GoToCoordinates(coordinateForPlayer);

            // get block type into hand
            // place block
            _mineFlayerDriver.PutBlockInInventory(block, 10);
            _mineFlayerDriver.PlaceBlock(block, neighbourBlock, placementVector);



        }

        [Given(@"there is the '(.*)' at X:(.*),Z:(.*) pointing to '(.*)'")]
        public void GivenThereIsTheAtXZPointingTo(string blockName, int x, int z, string targetBlockName)
        {
            var coordinateBlock = new Coordinate(x, _worldContext.YLevel, z);
            var block = new Block(coordinateBlock, blockName, GetBlockType(blockName));
            _worldContext.Blocks[blockName] = block;

            var targetBlock = _worldContext.Blocks[targetBlockName];

            var placementVector = new Vector(coordinateBlock.X - targetBlock.Coordinate.X, coordinateBlock.Y - targetBlock.Coordinate.Y, coordinateBlock.Z - targetBlock.Coordinate.Z);


            var coordinateForPlayer = coordinateBlock with { X = coordinateBlock.X + 3 * placementVector.X, Z = coordinateBlock.Z + 3 * placementVector.Z };
            GoToCoordinates(coordinateForPlayer);

            _mineFlayerDriver.PutBlockInInventory(block, 10);
            _mineFlayerDriver.PlaceBlock(block, targetBlock.Coordinate, placementVector);
        }

        [Given(@"there is the '(.*)' at X:(.*),Y:(.*),Z:(.*)")]
        public void GivenThereIsTheAtXYZ(string blockName, int x, int y, int z)
        {
            var coordinateBlock = new Coordinate(x, y, z);
            var neighbourBlock = coordinateBlock with { Y = coordinateBlock.Y - 1 };

            var placementVector = new Vector(0, 1, 0);

            var block = new Block(coordinateBlock, blockName, GetBlockType(blockName));
            _worldContext.Blocks[blockName] = block;


            // go to coord one coord near block
            GoToCoordinates(coordinateBlock with { X = coordinateBlock.X - 1 });

            // get block type into hand
            // place block
            _mineFlayerDriver.PutBlockInInventory(block, 10);
            _mineFlayerDriver.PlaceBlock(block, neighbourBlock, placementVector);
        }

        [When(@"a player puts an item into '(.*)'")]
        public void WhenAPlayerPutsAnItemInto(string p0)
        {
        }

        [Then(@"it appears in '(.*)'")]
        public void ThenItAppearsIn(string p0)
        {
        }


        private string GetBlockType(string blockname)
        {
            return blockname.Split('#')[0];
        }

        private void GoToCoordinates(Coordinate coordinateForPlayer)
        {
            _mineFlayerDriver.MovePlayerToCoordinates(coordinateForPlayer);

            bool stillMoving = true;
            do
            {
                var currentPlayerPosition = _mineFlayerDriver.GetPlayerCoordinate();

                if (Math.Abs(currentPlayerPosition.X - coordinateForPlayer.X) < 1 &&
                    Math.Abs(currentPlayerPosition.Y - coordinateForPlayer.Y) < 1 &&
                    Math.Abs(currentPlayerPosition.Z - coordinateForPlayer.Z) < 1)
                {
                    stillMoving = false;
                }

                Thread.Sleep(500);
            } while (stillMoving);
        }
    }



}
