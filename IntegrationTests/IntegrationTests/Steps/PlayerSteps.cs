using System;
using System.Collections.Generic;
using System.Text;
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
            var coordinateBlock = new Coordinate(x,_worldContext.YLevel, z);

            _worldContext.Blocks[blockName] = new Block(coordinateBlock, blockName, GetBlockType(blockName));


            var coordinateForPlayer = coordinateBlock with {X = coordinateBlock.X - 1};

            var lookingVector = new Vector(coordinateForPlayer.X - coordinateBlock.X,
                coordinateForPlayer.Y - coordinateBlock.Y,
                coordinateForPlayer.Z - coordinateBlock.Z);


            var currentPlayerPosition = _mineFlayerDriver.GetPlayerCoordinate();


            currentPlayerPosition = _mineFlayerDriver.MovePlayerToCoordinates(coordinateForPlayer);


            _mineFlayerDriver.LookAt(coordinateBlock);

            // get coords where block should be
            // go to coord one coord near block
            // look at targetblock
            // get block type into hand
            // place block

        }

        [Given(@"there is the '(.*)' at X:(.*),Z:(.*) pointing to '(.*)'")]
        public void GivenThereIsTheAtXZPointingTo(string blockName, int x, int z, string targetBlockName)
        {
            var coordinate = new Coordinate(x, _worldContext.YLevel, z);
            _worldContext.Blocks[blockName] = new Block(coordinate, blockName, GetBlockType(blockName));


        }

        [Given(@"there is the '(.*)' at X:(.*),Y:(.*),Z:(.*)")]
        public void GivenThereIsTheAtXYZ(string p0, int p1, int p2, int p3)
        {
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
    }

    

}
