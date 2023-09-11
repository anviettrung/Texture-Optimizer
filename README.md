# Fetch Google Sheet

Fetch data from Google Sheet to Unity.

- [Fetch Google Sheet](#fetch-google-sheet)
  * [Description](#description)
  * [Installation](#installation)
    - [Requirement](#requirement)
    - [Using Unity Package Manager](#using-unity-package-manager)
  * [Usage](#usage)
    + [2. Add tweens, intervals and callbacks to your Sequence](#2-add-tweens-intervals-and-callbacks-to-your-sequence)
  * [Example](#example)
  * [Documentation](#documentation)

> [Description](#description) 
| [Installation](#installation)
| [Usage](#usage)
| [Example](#example)
| [Documentation](DOCUMENTATION.md)
 
## Description
Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum

## Installation

#### Requirement

* Unity 2020.3 or later
* Package: Editor Coroutines ([com.unity.editorcoroutines](https://docs.unity3d.com/Packages/com.unity.editorcoroutines@1.0/manual/index.html))

#### Using Unity Package Manager

To use this in your Unity project import it from Unity Package Manager. You can [download it and import it from your hard drive](https://docs.unity3d.com/Manual/upm-ui-local.html), or [link to it from github directly](https://docs.unity3d.com/Manual/upm-ui-giturl.html).

> Git Url to install:
> https://github.com/anviettrung/UP-Fetch-Google-Sheet.git
> 
## Usage

<details>
  <summary> <b>static DOTween.Sequence()</b> </summary>

  > Returns a usable Sequence which you can store and add tweens to.
  > ```
  > Sequence mySequence = DOTween.Sequence();
  > ```
  
</details> 

### 2. Add tweens, intervals and callbacks to your Sequence
Note that all these methods need to be applied before the Sequence starts (usually the next frame after you create it, unless it's paused), or they won't have any effect.

Also note that any nested Tweener/Sequence needs to be fully created before adding it to a Sequence, because after that it will be locked.

Delays and loops (when not infinite) will work even inside nested tweens.

## Example

## Documentation

### 



