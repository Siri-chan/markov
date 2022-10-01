# markov
A markov-chain generator.
Uses `.NET 6.0`, but should backport to at least `.NET 4.0`

## Running
This program expects a `list.txt` file in the in the `%LOCALAPPDATA%/.markov` or `~/.markov` directories (*or wherever you have changed the working directory to* (See [Configuration](#configuration))) in order to function.
Follow the instructions in `test_list.txt`, or supply your own `\n` seperated list of plain text.

This program can then be run using ther most recent version of the `.NET` SDK, and running the command `dotnet run`.

## Configuration
You can use the ConfigBuilder object at the top of `Program.cs` to change the working directory (where `.cache` and `list.txt` go), disable caching, or the length of each quote-chunk generated.

## TroubleShoooting
This program will cache prior results in the directory of the text list, meaning that for each run with the same list, the generations get faster and faster.
For the test list (~150 lines), the cache can reach up to `100,000 bytes` and will exist in the `%LOCALAPPDATA%/.markov` or `~/.markov` directories (&or wherever you have changed the working directory to& (See [Configuration](#configuration))), depending on platform.
You can

## Examples
These examples were generated using 10 words and the test_list.
They are "emotional" quotes.

```txt
Take up and within us are dirty, the people you strong.
```
```txt
Don't settle. As you up everything around us becomes better person.
```

## License
Copyright (c) Kira "Siri" K, 2022.
Licensed under the Mozilla Public License (MPL).
Breach of this license is illegal under international Copyright law.
See `LICENSE` for more details.
