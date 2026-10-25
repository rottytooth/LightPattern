# Original Light Pattern project (2012)

> [!WARNING]
> This is the original 2012 reference compiler for Light Pattern. It does **not** match the current version of the language. Programs written for the current spec may not compile or run correctly here.

Upgraded to Framework 4.8



## About

Light Pattern is an esoteric programming language by Daniel Temkin (2012) where the source code is a series of photographs. The compiler ignores what the photos show. It reads only each photo's exposure and dominant color. A program is a directory of JPEGs, read in alphabetical order, so a program can be hidden in an ordinary-looking photo collection.

The language is imperative and Turing complete.

More: [Light Pattern on the Esolang wiki](https://esolangs.org/wiki/Light_Pattern)

## How instructions are encoded

Each instruction is a three-digit ternary code. Each digit is the change between one photo and the next in one of three attributes: **color**, **aperture**, and **shutter speed**.

- `1` means no change.
- For aperture and shutter speed, `2` means more light (larger aperture or slower shutter) and `0` means less light.
- Color cycles Red → Green → Blue → Red. A forward step is `2` and a backward step is `0`.

Aperture and shutter speed come from the EXIF data. The dominant color is found by tallying the RGB values of each pixel.

### Commands

| Code | Command |
|------|---------|
| 200 | Let (variable name, then expression) |
| 201 | Declare (variable name, then type) |
| 202 | While (expression, then commands) |
| 210 | End While |
| 211 | If (expression, then commands) |
| 212 | Else |
| 220 | End If |
| 221 | Print |
| 222 | Reset (ignore the next photo, used when shutter or aperture would go out of range) |
| 001 | Fork |

### Types (used only in declarations)

| Code | Type |
|------|------|
| 001 | Int |
| 010 | Char |
| 011 | Double |
| 012 | String |

### Expressions

| Code | Expression |
|------|------------|
| 100 | Variable |
| 101 | Int (+) |
| 102 | Int (−) |
| 110 | Char |
| 111 | Double (+) |
| 112 | Double (−) |
| 120 | Begin String |
| 000 | End String |
| 020 | Two-code expression (followed by an operator code: comparisons, boolean logic, parentheses, arithmetic) |

## Writing a program

1. Write the ternary codes in a `.lpc` file, one per line, with optional `//` comments.
2. Run the compiler with `/g` to turn the codes into a `.lp` file of suggested exposure and color settings. Add `222` resets wherever the settings go out of range.
3. Compile the `.lp` file with `/n` to test it without photos. Add `/c` to output C# instead of an executable.
4. When the program works, take the photos as listed and compile the resulting directory.

### Compiler options

| Flag | Effect |
|------|--------|
| `/c` | Generate a C# file instead of an executable |
| `/n` | Compile from a `.lp` list of exposures instead of photos |
| `/g` | Translate a `.lpc` list of codes into suggested exposures |
| `/t` | Ternarize a binary file into exposures plus a `.cpp` file for Arduino |
| `/v` | Verbose: print the deltas as the compiler reads them (on by default for images) |
| `/h`, `/?` | Print help |

## Further reading

- [lightpattern.info](http://lightpattern.info), including [Hello, World examples](http://lightpattern.info/Basics/HelloWorld)
- Daniel Temkin, (http://www.icp.org/perspective/light-pattern-building-an-algorithmic-photography)["Light Pattern: Writing Code with Photographs," *Leonardo*], 2016
- Astrid Bugdor, (https://killscreen.com/articles/kaleidoscopic-coding-programming-language-light-pattern/)[*Kill Screen Daily*], 2014
