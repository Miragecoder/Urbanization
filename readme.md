# Urbanization

AppVeyor: [![Build status](https://ci.appveyor.com/api/projects/status/ala0uiruj1s644pq/branch/master?svg=true)](https://ci.appveyor.com/project/Miragecoder/urbanization/branch/master)

Urbanization is a .NET implementation of a city building simulation game, heavily inspired by the classic [Simcity](http://en.wikipedia.org/wiki/SimCity_(1989_video_game)) game released in 1989; of which its source code has been released under the title [Micropolis](https://github.com/SimHacker/micropolis).

The implementation is far from complete, but most of the basics are currently present. These include (amongst others):
- Customizable terrain generation
- Power grid and infrastructure simulation (including population growth)
- Crime and pollution

![Screenshot of 'Urbanization'](/screenshot.png?raw=true "Screenshot of 'Urbanization'")

## Installation

1. Clone the repository
2. Build the solution
3. Run the (primarily, integration) tests
4. Run the *Mirage.Urbanization.WinForms*-project 

## Usage

Currently, the simulation engine has only been implemented in a Winforms client. Run the *Mirage.Urbanization.WinForms*-project and use the application by intuition like any other windows application.

## Contributing

I'm not expecting high contribution rates on this project but if you're totally interested you're more than welcome to chime in.

## History

This project was set up primarily for educational purposes. It was primarily born out of interest into (game) engine design. To its author, it is a welcome expedition away from his day-to-day activities as a .NET developer which primarily involve the development of data synchronization services.

## Credits

#### SharpDX:

http://sharpdx.org/

#### RhinoMocks:

http://hibernatingrhinos.com/oss/rhino-mocks

## License

This project uses the MIT license. See 'license.txt' for more info.
