const asyncHandler = require('express-async-handler')
const { Vec3 } = require('vec3')
const mineflayer = require('mineflayer')
const Item = require("prismarine-item")("1.8");
const { pathfinder, Movements } = require('mineflayer-pathfinder')
const { GoalNear, GoalBlock, GoalXZ, GoalY, GoalInvert, GoalFollow } = require('mineflayer-pathfinder').goals


const options = {
  host: 'localhost',
  port: 25565,
  username: 'SpecFlow'
}

const bot = mineflayer.createBot(options);
bot.on('error', (err) => {
  console.log(err);
});

bot.loadPlugin(pathfinder);

bot.once('spawn', () => {
    // Once we've spawn, it is safe to access mcData because we know the version
    mcData = require('minecraft-data')(bot.version)
  
    // We create different movement generators for different type of activity
    defaultMove = new Movements(bot, mcData)

    var items = mcData.itemsArray;
    var i;
    for (i = 0; i < items.length; i++) {
        console.log(items[i].name);
    }
    
    
  });
  