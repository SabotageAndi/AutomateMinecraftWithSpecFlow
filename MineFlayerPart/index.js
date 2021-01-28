const asyncHandler = require('express-async-handler')
const { Vec3 } = require('vec3')
const mineflayer = require('mineflayer')
const Item = require("prismarine-item")("1.8");
const { pathfinder, Movements } = require('mineflayer-pathfinder')
const { GoalNear, GoalBlock, GoalXZ, GoalY, GoalInvert, GoalFollow } = require('mineflayer-pathfinder').goals

const express = require('express');
const app = express();
const port = 3000;

const options = {
  host: 'localhost',
  port: 25565,
  username: 'SpecFlow'
}

const bot = mineflayer.createBot(options);
bot.on('error', (err) => {
  console.log(err);
});

app.use(express.urlencoded());
app.use(express.json());

app.listen(port, () => {
  console.log(`Example of a server running at http://localhost:${port}`)
});


let mcData;
let defaultMove;

app.get('/position', (req, res) => {
  res.send(bot.entity.position);
})

app.post('/position', (req, res) => {
  bot.pathfinder.setMovements(defaultMove);
  bot.pathfinder.setGoal(new GoalBlock(req.body.x, req.body.y, req.body.z));
  res.send(bot.entity.position);
});

app.post('/look', asyncHandler(async (req, res, next) => {
  await bot.lookAt(req.body);
}));

app.get('/inventory', (req, res) => {
  res.send(sayItems());
});

app.post('/slot', asyncHandler(async (req, res, next) => {
  var slotPosition = req.body.position;
  var itemName = req.body.item;

  item = findItemByName(itemName);

  await bot.creative.setInventorySlot(slotPosition, item);

  res.sendStatus(200);
}));


app.post('/placeBlock', asyncHandler(async (req, res, next) => {
    var itemName = req.body.item;
    var position = new Vec3(req.body.position.x, req.body.position.y, req.body.position.z);

    await equipItem(itemName, 'hand');

    const referenceBlock = bot.blockAt(position);
    await bot.placeBlock(referenceBlock, new Vec3(0, 1, 0));

    res.sendStatus(200);

}));

bot.loadPlugin(pathfinder)

bot.once('spawn', () => {
  // Once we've spawn, it is safe to access mcData because we know the version
  mcData = require('minecraft-data')(bot.version)

  // We create different movement generators for different type of activity
  defaultMove = new Movements(bot, mcData)
})


function sayItems(items = bot.inventory.items()) {
  const output = items.map(itemToString).join(', ')
  if (output) {
    bot.chat(output)
  } else {
    bot.chat('empty')
  }
}

function findItemByName(itemName) {
  var items = mcData.itemsArray;
  var i;
  for (i = 0; i < items.length; i++) {
    if (items[i].displayName === itemName) {
      var foundItem = items[i];

      return new Item(foundItem.id, 1);
    }
  }
  return null;
}

async function equipItem(name, destination) {
  const item = findItemInInventoryByName(name)
  if (item) {
    try {
      await bot.equip(item, destination)
      bot.chat(`equipped ${name}`)
    } catch (err) {
      bot.chat(`cannot equip ${name}: ${err.message}`)
    }
  } else {
    bot.chat(`I have no ${name}`)
  }
}

async function unequipItem(destination) {
  try {
    await bot.unequip(destination)
    bot.chat('unequipped')
  } catch (err) {
    bot.chat(`cannot unequip: ${err.message}`)
  }
}

function useEquippedItem() {
  bot.chat('activating item')
  bot.activateItem()
}

async function craftItem(name, amount) {
  amount = parseInt(amount, 10)
  const mcData = require('minecraft-data')(bot.version)

  const item = mcData.findItemOrBlockByName(name)
  const craftingTableID = mcData.blocksByName.crafting_table.id

  const craftingTable = bot.findBlock({
    matching: craftingTableID
  })

  if (item) {
    const recipe = bot.recipesFor(item.id, null, 1, craftingTable)[0]
    if (recipe) {
      bot.chat(`I can make ${name}`)
      try {
        await bot.craft(recipe, amount, craftingTable)
        bot.chat(`did the recipe for ${name} ${amount} times`)
      } catch (err) {
        bot.chat(`error making ${name}`)
      }
    } else {
      bot.chat(`I cannot make ${name}`)
    }
  } else {
    bot.chat(`unknown item: ${name}`)
  }
}

function itemToString(item) {
  if (item) {
    return `${item.name} x ${item.count}`
  } else {
    return '(nothing)'
  }
}

function findItemInInventoryByName(name) {
  return bot.inventory.items().filter(item => item.name === name)[0]
}

function printError(err) {
  bot.chat(err);
}

