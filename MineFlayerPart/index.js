const mineflayer = require('mineflayer')
const { pathfinder, Movements } = require('mineflayer-pathfinder')
const { GoalNear, GoalBlock, GoalXZ, GoalY, GoalInvert, GoalFollow } = require('mineflayer-pathfinder').goals

const express = require('express');
const app = express();
const port = 3000;

const options = {
    host: 'localhost',
    port: 53608,
    username: 'SpecFlow'
  }
  
const bot = mineflayer.createBot(options);

app.use(express.urlencoded()); 
app.use(express.json());

app.listen(port, () => {
    console.log(`Example of a server running at http://localhost:${port}`)
});

app.get ('/position', (req, res) => {
    res.send(bot.entity.position);
})

app.post('/position', (req, res) => {
    const mcData = require('minecraft-data')(bot.version)
    const defaultMove = new Movements(bot, mcData)
    bot.pathfinder.setMovements(defaultMove);
    bot.pathfinder.setGoal(new GoalBlock(req.body.x, req.body.y, req.body.z));
    res.send(bot.entity.position);
});

bot.loadPlugin(pathfinder)

bot.once('spawn', () => {
  // Once we've spawn, it is safe to access mcData because we know the version
  const mcData = require('minecraft-data')(bot.version)

  // We create different movement generators for different type of activity
  const defaultMove = new Movements(bot, mcData)

  bot.on('path_update', (r) => {
    const nodesPerTick = (r.visitedNodes * 50 / r.time).toFixed(2)
    console.log(`I can get there in ${r.path.length} moves. Computation took ${r.time.toFixed(2)} ms (${r.visitedNodes} nodes, ${nodesPerTick} nodes/tick)`)
  })

  bot.on('goal_reached', (goal) => {
    console.log('Here I am !')
  })

  bot.on('chat', (username, message) => {
    if (username === bot.username) return

    const target = bot.players[username] ? bot.players[username].entity : null
    if (message === 'come') {
      if (!target) {
        bot.chat('I don\'t see you !')
        return
      }
      const p = target.position

      bot.pathfinder.setMovements(defaultMove)
      bot.pathfinder.setGoal(new GoalNear(p.x, p.y, p.z, 1))
    } else if (message.startsWith('goto')) {
      const cmd = message.split(' ')

      if (cmd.length === 4) { // goto x y z
        const x = parseInt(cmd[1], 10)
        const y = parseInt(cmd[2], 10)
        const z = parseInt(cmd[3], 10)

        bot.pathfinder.setMovements(defaultMove)
        bot.pathfinder.setGoal(new GoalBlock(x, y, z))
      } else if (cmd.length === 3) { // goto x z
        const x = parseInt(cmd[1], 10)
        const z = parseInt(cmd[2], 10)

        bot.pathfinder.setMovements(defaultMove)
        bot.pathfinder.setGoal(new GoalXZ(x, z))
      } else if (cmd.length === 2) { // goto y
        const y = parseInt(cmd[1], 10)

        bot.pathfinder.setMovements(defaultMove)
        bot.pathfinder.setGoal(new GoalY(y))
      }
    } else if (message === 'follow') {
      bot.pathfinder.setMovements(defaultMove)
      bot.pathfinder.setGoal(new GoalFollow(target, 3), true)
      // follow is a dynamic goal: setGoal(goal, dynamic=true)
      // when reached, the goal will stay active and will not
      // emit an event
    } else if (message === 'avoid') {
      bot.pathfinder.setMovements(defaultMove)
      bot.pathfinder.setGoal(new GoalInvert(new GoalFollow(target, 5)), true)
    } else if (message === 'stop') {
      bot.pathfinder.setGoal(null)
    }
  })
})