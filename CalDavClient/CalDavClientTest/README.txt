In order to run these tests you have to start the Radicale Server (http://radicale.org/) and set following AppSetting in App.config:
'radicaleCollectionDir' - path where calendars of unit tests are stored, has to be same as in './src/CalDavClient/Radicale/radicale_config.txt' under section [storage]

You can start the Radicale Server by executing './src/CalDavClient/run_radicale.bat'

Radicale requires 'python' to be installed (https://www.python.org/downloads/).

2014-06-26: Radicale does not support nested filters (see: https://github.com/Kozea/Radicale/issues/33)