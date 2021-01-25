const mineflayer = require('mineflayer')
const Item=require("prismarine-item")("1.8");
const { pathfinder, Movements } = require('mineflayer-pathfinder')
const { GoalNear, GoalBlock, GoalXZ, GoalY, GoalInvert, GoalFollow } = require('mineflayer-pathfinder').goals

const express = require('express');
const app = express();
const port = 3000;

const options = {
    host: 'localhost',
    port: 58914,
    username: 'SpecFlow'
  }
  
const bot = mineflayer.createBot(options);

app.use(express.urlencoded()); 
app.use(express.json());

app.listen(port, () => {
    console.log(`Example of a server running at http://localhost:${port}`)
});


let mcData;
let defaultMove;

app.get ('/position', (req, res) => {
    res.send(bot.entity.position);
})

app.post('/position', (req, res) => {
    bot.pathfinder.setMovements(defaultMove);
    bot.pathfinder.setGoal(new GoalBlock(req.body.x, req.body.y, req.body.z));
    res.send(bot.entity.position);
});

app.get ('/inventory', (req, res) => {
  res.send(sayItems());
});

app.post ('/slot', (req, res) => {
    var slotPosition = req.body.position;
    var itemName = req.body.item;

    item = findItemByName(itemName);

    bot.creative.setInventorySlot(slotPosition, item);

    res.sendStatus(200);
});

bot.loadPlugin(pathfinder)

bot.once('spawn', () => {
  // Once we've spawn, it is safe to access mcData because we know the version
  mcData = require('minecraft-data')(bot.version)

  // We create different movement generators for different type of activity
  defaultMove = new Movements(bot, mcData)

  bot.on('path_update', (r) => {
    const nodesPerTick = (r.visitedNodes * 50 / r.time).toFixed(2)
    console.log(`I can get there in ${r.path.length} moves. Computation took ${r.time.toFixed(2)} ms (${r.visitedNodes} nodes, ${nodesPerTick} nodes/tick)`)
  })
})


function sayItems (items = bot.inventory.items()) {
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
    if (items[i].displayName === itemName)
    {
      var foundItem = items[i];

      return new Item(foundItem.id, 1);
    }
  }
  return null;
}

function tossItem (name, amount) {
  amount = parseInt(amount, 10)
  const item = itemByName(name)
  if (!item) {
    bot.chat(`I have no ${name}`)
  } else if (amount) {
    bot.toss(item.type, null, amount, checkIfTossed)
  } else {
    bot.tossStack(item, checkIfTossed)
  }

  function checkIfTossed (err) {
    if (err) {
      bot.chat(`unable to toss: ${err.message}`)
    } else if (amount) {
      bot.chat(`tossed ${amount} x ${name}`)
    } else {
      bot.chat(`tossed ${name}`)
    }
  }
}

async function equipItem (name, destination) {
  const item = itemByName(name)
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

async function unequipItem (destination) {
  try {
    await bot.unequip(destination)
    bot.chat('unequipped')
  } catch (err) {
    bot.chat(`cannot unequip: ${err.message}`)
  }
}

function useEquippedItem () {
  bot.chat('activating item')
  bot.activateItem()
}

async function craftItem (name, amount) {
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

function itemToString (item) {
  if (item) {
    return `${item.name} x ${item.count}`
  } else {
    return '(nothing)'
  }
}

function itemByName (name) {
  return bot.inventory.items().filter(item => item.name === name)[0]
}

function printError(err)
{
  bot.chat(err);
}