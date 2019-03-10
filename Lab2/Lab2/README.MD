# TASK:

VARIANT NO 7

![dan](https://user-images.githubusercontent.com/24621285/54087170-11b90800-4359-11e9-99ed-c8d2ae34f496.PNG)

## What was done?

Were created separated arrays of signals with 2 values each because diagram is like a tree, we have parents and children.
Order of starting the threads doesn't matter, the parents will wait till both children finish their activity then will launch themselves.
This is done by using AutoReset. It works as a ticket, one way available. After thread its done, AutoReset resets its value to false again.

See code [here](https://github.com/Secoranda/PR/blob/master/Lab2/Lab2/Lab2/Program.cs)
