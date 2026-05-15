# rc-cloud.de

This is hosted at https://rc-cloud.de.

It aggregates German RC racing dates from [DMC](https://dmc-online.com/), [Myrcm](https://myrcm.ch), [RC Car Online](https://rccar-online.de), [RCK-Kleinserie](https://kleinserie.rck-solutions.de/indexgo.php), [RCK-Challenge](https://challenge.rck-solutions.de/indexgo.php) and [LRP Offroad Series](https://lrp.cc).

It also has limited support for BeNeLux RC racing dates.

## Technical stuff

The frontend is an Angular SPA served as a Docker container (nginx). The backend is an ASP.NET Core Web API (`RcCloud.WebApi`) also deployed as a Docker container. Both images are published to GitHub Container Registry and deployed via webhook.

It uses Domain Driven Design principles and makes effort to create clean and maintainable code.

Data is stored in MongoDB. The canonical club database lives in `db/club-db.json`.
