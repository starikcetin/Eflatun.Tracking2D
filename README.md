This repository has UPM support. To install it, paste the following into your manifest's dependencies (Packages/manifest.json):

    "com.coffee.git-dependency-resolver": "https://github.com/mob-sakai/GitDependencyResolverForUnity.git#1.1.2",
    "com.coffee.upm-git-extension": "https://github.com/mob-sakai/UpmGitExtension.git#0.9.1",
    "com.starikcetin.eflatun.tracking2d": "https://github.com/starikcetin/Eflatun.Tracking2D.git#0.0.2",

# Eflatun.Tracking2D #

This was an attempt to replace Unity's built-in 2D physics engine wtih only the features I wanted.

**As of right now, this code is not throughly tested and doesn't have a shiny performance.**

I *may* convert this library to use Unity ECS once it becomes production-ready, but it is not certain.

Dependencies
---
- [Eflatun.Expansions](https://github.com/starikcetin/Eflatun.Expansions)
- [Eflatun.UnityCommon](https://github.com/starikcetin/Eflatun.UnityCommon)

License
---
MIT license. Refer to the [LICENSE](https://github.com/starikcetin/Eflatun.Tracking2D/blob/master/LICENSE) file.

Copyright (c) 2018 S. Tarık Çetin
