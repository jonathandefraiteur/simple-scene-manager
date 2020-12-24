# To Install

## Via scoped registries
Setup your scoped registry,
then add `com.jonathan-defraiteur.unity.simple-scene-manager` to your dependencies:

```
# ./Packages/manifest.json
{
  "dependencies": {
    "com.jonathan-defraiteur.unity.simple-scene-manager": "latest"
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

## Via gitlab url
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
