# Farley File
## This project you see not!

Farleyfile is an information management system that uses Lokad.CQRS with a dash of event sourcing, DDD and "cloud magic".

> Note, that this project is currently developed using only _Lokad.CQRS.Portable.dll_ in a fully-persisted scenario. On other words, it does not require any Azure dependencies (but could still be ported to the cloud later).

Unlike other CQRS samples available out there, I'm developing it for a real use. It is supposed to help me to manage notes, ideas, articles and projects in a way that is native to my thinking process (read about [Farley File](http://en.wikipedia.org/wiki/Farley_File)). That's something I always wanted to have, but always failed to implement.

Currently I'm still evolving the functionality and features (stories, contexts, note management etc). Because of that, the actual codebase might be a little bit confusing and hacky - not suitable for learning or presenting concepts of Lokad.CQRS (i.e.: aggregate roots and tests are messed up).

So if you accidentally stumble across this project, I recommend to avoid diving into it right now. This could make everything feel much more complex than it actually is. Besides, there are a few really cool features I'll be adding only towards the end (to keep the surprise :) Some of the design choices will not make a lot of sense till then.

## Roadmap

The project itself aims to accomplish two objective:

* Become a **tool for my information management** (I'd be glad if it helps somebody else, but this is not goal).
* Serve as a **practical Lokad.CQRS show-case with DDD/Cloud/Event sourcing**, that deals with all the production level details which are often discarded from the sample projects (evolving, testing, configuration, contracts, maintenance etc).

These objectives form the first milestone. When they are reached, the project will be officially published with all the necessary cleaning and comments.

**If** the first milestone is reached, then I think about starting a community effort to **create a show-case of building Lokad.CQRS clients on different platforms**. I'm thinking of something like: RoR, ASP.NET MVC, Windows Phone and iPhone, while going for iCloud-style replication and [continuous client](http://kellabyte.com/2011/05/26/continuous-client-our-multi-device-dream-but-how-do-we-build-it/) experience. All of these scenarios are rather straightforward to achieve within a single architecture (although it might be boring to have the same design on multiple platforms) yet there should be some platform-specific quirks.

I want to highlight similarities and help to establish common body of knowledge for further development. There are a few devs that are already interested in practicing and extending their skillset to this area, but **if you want to join - just get in touch**.


## Release Date

Publishing of this sample is planned closer towards the end of August - beginning of September, when we'll be releasing v3 refresh of Lokad.CQRS.

Meanwhile, you can just enjoy the summer and stay tuned to my blog. You can also grab a copy of DDD book by Eric Evans, practice vim or check out LMAX implementations.

Best regards,

**Rinat Abdullin** | Technology Leader at [Lokad.com](http://lokad.com) | Writer at [Abdullin.com](http://abdullin.com) | [Contacts](http://abdullin.com/contact/)
