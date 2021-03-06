Windows Phone Bowling Calculator 
================================

A [Windows Phone application](http://www.windowsphone.com/s?appid=9287a99a-6068-4ff4-925b-988c8c99dbf6) that calculates the scores for multiple players in a bowling game.

Created by [Kamran Ayub](http://kamranicus.com) as a production app and to use as a reference
on building Windows Phone applications using common patterns & practices.

You can download the Bowling Calculator app by [visting the Windows Phone store](http://www.windowsphone.com/s?appid=9287a99a-6068-4ff4-925b-988c8c99dbf6).

![Game in progress](/BowlingCalculator/Assets/WXGA/wxga-game-in-progress.png)

Features
========

* Dead simple to use
* Support for multiple players
* Supports resuming a game if you leave the app
* Supports resetting the game or removing all players to start over
* English and French localization

Contributing
============

Please feel free to send a pull request or open an issue for any bugs/enhancements. This app is not meant to
track games over time, it's very focused on the use case of calculating a game's score. There are other apps
that track games over time or provide stats or more features.

Building
========

## Prerequisites

* Microsoft Visual Studio 2012 or above
* Windows Phone 8.0 SDK

You will need to create a RESX file under `Resources` called `AppSecrets.resx` and add a string property
for `BugSenseApiKey` in order to build. You do not need an actual API key to run locally in Debug mode.

License
=======

Licensed under MS-PL. See [License](LICENSE.md).

I would kindly ask that you refrain from publishing a derivative app in the store.
Instead, please consider contributing back to the project with enhancements or bug fixes.
