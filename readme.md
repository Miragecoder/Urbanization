# Urbanization

[![Build status](https://ci.appveyor.com/api/projects/status/ala0uiruj1s644pq/branch/master?svg=true)](https://ci.appveyor.com/project/Miragecoder/urbanization/branch/master)

Urbanization is a .NET implementation of a city building simulation game, heavily inspired by the classic [Simcity](http://en.wikipedia.org/wiki/SimCity_(1989_video_game)) game released in 1989; of which its source code has been released under the title [Micropolis](https://github.com/SimHacker/micropolis).

Among the features offered by this implementation are:
- Customizable terrain generation
- Power grid and infrastructure simulation (including population growth and commuting)
- Crime and fire hazard, both of which can be controlled by building police and/or fire stations
- Pollution, which is negatively influenced by industrial areas (and positively by nature!)
- Overlay heat maps; offering insight into the failing or succesful growth of city areas.

![Screenshot of 'Urbanization'](/screenshot.png?raw=true "Screenshot of 'Urbanization'")

## Demo

A live demo of the web app is hosted at:
http://urbanization.cloudapp.net/ 

Note: Availability and performance may vary. If the link doesn't work, you can always grab the latest build:https://ci.appveyor.com/project/Miragecoder/urbanization/build/artifacts

## Installation

1. Clone the repository
2. Build the solution
3. Run the (primarily, integration) tests
4. Run the *Mirage.Urbanization.WinForms*-project (if you wish to run from your desktop)
5. Run the *Mirage.Urbanization.Web*-project (if you wish to host a web server)

## Usage

Run the *Mirage.Urbanization.WinForms*-project and use the application by intuition like any other windows application. The *Mirage.Urbanization.Web*-project can be utilized if you wish to run the web version of the game. Starting the *web*-project will immediately start a web server on all network devices, on port 80. (Http)

You can also start a web server from within an instance of *Mirage.Urbanization.WinForms*; allowing access to the game session that is taking place inside the desktop app from the web.

## Contributing

The project is not completely finished, but it's well into its beta stage. I'm not expecting high contribution rates on this project but if you're totally interested you're more than welcome to chime in.

## History

This project was set up primarily for educational purposes. It was primarily born out of interest into (game) engine design. To its author, it is a welcome expedition away from his day-to-day activities as a .NET developer which primarily involve the development of data synchronization services.

## Credits

#### SharpDX:

http://sharpdx.org/

#### RhinoMocks:

http://hibernatingrhinos.com/oss/rhino-mocks

#### System.Collections.Immutable:

http://blogs.msdn.com/b/bclteam/p/immutable.aspx

#### SignalR:

http://signalr.net/

#### JQuery:

http://jquery.com/

#### JQuery(UI):

https://jqueryui.com/

#### Topshelf:

https://topshelf-project.com/

#### accounting.js:

http://openexchangerates.github.io/accounting.js/

## License

This project uses the MIT license. See 'license.txt' for more info.
