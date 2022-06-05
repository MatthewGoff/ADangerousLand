---
uid: AddingArticles
---

# Adding Articles

Basically articles are markdown files in the docfx_project/articles folder. The only new thing is that you should include a uid (unique identifier) at the top:

```
---
uid: MyArticle
---

# Article Header

Article content
```

Once you've created your article you need to modify the table of contents so that the article will appear in the sidebar. To do this open docfx_project/articles/toc.yml and modify the table of contents (toc). The syntax should be evident from the file.

To be able to view your changes you need to rebuild the website and then host locally or push to github. See: [Using DocFX](xref:UsingDocFX).

For further information visit the [DocFX tutorials](https://dotnet.github.io/docfx/tutorial/docfx_getting_started.html) (actually I find its easier to just google).