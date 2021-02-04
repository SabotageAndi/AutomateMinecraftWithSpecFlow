# Challenges for Black box testing and how they are solved

## How to control the Black box from the outside?

For Minecraft: mineflayer Javascript library to write a bot

For Webapplications:

- Selenium
- Playwright

For Mobile Applications:

- Appium

For Desktop Applications:

- Windows AppDriver (https://github.com/Microsoft/WinAppDriver)

For APIs:

- RestSharp

## We need to start the black box and know when it is ready

For Minecraft: started process, and waited for a specific output

For Webapplication & HTTP/Json APIs:

- start an in process webserver
    - wait until this is up and running
- start IIS
    - ping a status endpoint?
    - ping a endpoint that doesn't have sideeffects

For Mobile Applications:

- done by Appium

## Timing issues

- Start with sleeps methods
- convert to polls with a max runtime
- events to see, when it is ready

## Resetting your environment

For Minecraft: restoring a good copy of the world

For Database:
    
    - restore a good state of the DB from a backup
    - copy the db files
    - undo all the changes you did
    - start with an empty db and fill everything everytime

For Files:

    - restore a good copy

## Special configuration to make it work

For Minecraft:
    - make server offline

For API:
    - mock the other services it is calling

For WebApplications:
    - SSL Certs
    - mock external dependency

## Double State

Because you can't access the state, you have to save your own "copy"

## Shutdown of the blackbox

For Minecraft:
    - stopping the two process in the right order

For Webapplication:
    - are all requests done?
    - are there any transactions open?
    - does a process kill corrupt some data?