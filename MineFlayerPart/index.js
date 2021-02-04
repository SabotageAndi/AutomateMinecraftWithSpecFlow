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

  bot.creative.setInventorySlot(slotPosition, item);


  res.sendStatus(200);
}));


app.post('/placeBlock', asyncHandler(async (req, res, next) => {
  var itemName = req.body.item;
  var position = new Vec3(req.body.position.x, req.body.position.y, req.body.position.z);
  var placementVector = new Vec3(req.body.placementVector.x, req.body.placementVector.y, req.body.placementVector.z);

  await equipItem(itemName, 'hand');

  const referenceBlock = bot.blockAt(position);

  bot.setControlState('sneak', true);
  await bot.placeBlock(referenceBlock, placementVector);

  bot.setControlState('sneak', false);
  await sleep(100);
  res.sendStatus(200);

}));

app.get('/chest', asyncHandler(async (req, res, next) => {
  var chestPosition = new Vec3(req.query.x, req.query.y, req.query.z);

  const chestBlock = bot.blockAt(chestPosition);

  const chest = bot.openChest(chestBlock);
  await sleep(2000);

  try {
    var items = chest.items();
  } catch (error) {
    res.send(error);
  }
  finally {
    chest.close();
  }

  res.send(items);
}));

app.post('/chest', asyncHandler(async (req, res, next) => {
  var chestPosition = new Vec3(req.body.position.x, req.body.position.y, req.body.position.z);

  const chestBlock = bot.blockAt(chestPosition);

  const chest = bot.openChest(chestBlock);
  await sleep(500);


  await depositItem(chest, req.body.item, req.body.amount);

  chest.close();
  res.sendStatus(200);
}));

app.get('/status', (req, res) => 
{
  if (mcData != undefined)
  {
    res.sendStatus(200);
  }
  else
  {
    res.sendStatus(503);
  }
});

bot.loadPlugin(pathfinder);

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
  const item = findItemInInventoryByDisplayName(name)
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

function itemToString(item) {
  if (item) {
    return `${item.displayName} x ${item.count}`
  } else {
    return '(nothing)'
  }
}

function findItemInInventoryByDisplayName(name) {
  return bot.inventory.items().filter(item => item.displayName === name)[0]
}

function printError(err) {
  bot.chat(err);
}


async function depositItem(chest, name, amount) {
  const item = findItemInInventoryByDisplayName(name)
  if (item) {
    try {
      await chest.deposit(item.type, null, amount)
      bot.chat(`deposited ${amount} ${item.name}`)
    } catch (err) {
      bot.chat(`unable to deposit ${amount} ${item.name} because of ${err}`)
    }
  } else {
    bot.chat(`unknown item ${name}`)
  }
}

function sleep(ms) {
  return new Promise((resolve) => {
    setTimeout(resolve, ms);
  });
}   