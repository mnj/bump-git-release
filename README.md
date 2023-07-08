# bump-git-release

Helper util, to just bump a git tag, to solve a very specific use case, that i needed personally.

For use in repos with a dev/main branch, that represent dev/prod environments, and that are publishing nuget packages.

This basically just does:

```
    git checkout main
    git pull
    git merge -X theirs dev
    git tag -a {newVersion} -m "new release"
    git push --follow-tags
    git checkout dev
```

And increments whatever the latest tag is by one minor version.

Since this is .net 8, we can just build a AOT build, that runs pretty much instant, is self contained, just throw the
binary in the path somewhere.