# rc-cloud.de

This is hosted at https://rc-cloud.de.

It retrieves German RC racing dates from [DMC](https://dmc-online.com/), [Myrcm](https://myrcm.ch), [RC Car Online](https://rccar-online.de), [RCK-Kleinserie](https://kleinserie.rck-solutions.de/indexgo.php) and [RCK-Challenge](https://challenge.rck-solutions.de/indexgo.php).

At the moment it also has limited support for BeNeLux RC racing dates.

## Technical stuff

It is intended to deploy on Azure Static Web App although there should be nothing in the way of hosting it the classic way.

It uses Domain Driven Design principles and makes effort to create clean and maintainable code.

It uses Angular in the frontend. The backend is C# and Azure Functions. Furthermore a CLI exists for development purposes, but cannot be regarded as stable.
Data is stored in JSON files and MongoDB.
