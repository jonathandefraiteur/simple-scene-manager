# Simple Scene Manager v1.3.1
A minimalistic tool to help load and manage scenes.

## To Install

### Via scoped registries
Setup your scoped registry,
then add `com.jonathan-defraiteur.unity.simple-scene-manager` to your dependencies:

```
# ./Packages/manifest.json
{
  "dependencies": {
    "com.jonathan-defraiteur.unity.simple-scene-manager": "1.3.1"
    ...
  },
  "scopedRegistries": [
    {
      "name": "NPM JS Registry",
      "url": "https://registry.npmjs.org/",
      "scopes": [
        "com.jonathan-defraiteur"
      ]
    }
  ],
}
```

### Via GitLab url
Add the following to your dependencies:
```
# ./Packages/manifest.json
{
  "dependencies": {
    "com.jonathan-defraiteur.unity.simple-scene-manager": "https://gitlab.com/jonathan-defraiteur-projects/unity/simple-scene-manager.git",
    ...
  }
}
```

### Via GitHub url
Add the following to your dependencies:
```
# ./Packages/manifest.json
{
  "dependencies": {
    "com.jonathan-defraiteur.unity.simple-scene-manager": "https://github.com/jonathandefraiteur/simple-scene-manager.git",
    ...
  }
}
```
