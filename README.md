# Description
Here be ~~dragons~~ examples to implement Logical Clock from this [magical book](https://www.amazon.com/Distributed-Computing-Principles-Algorithms-Systems/dp/0521189845)

I use RX.NET to mimic a distributed system. Code's ugly, yeah well what else is new.

## Model
 - Three processes, streaming events with twisted timestamps (like from other machines)
 - Each process have a 50% chance to publish an event to another. You can control the amount of republishing you want to happen.

## Scalar Clock
 - Eventual consistency happens
 - No causal consistency happens

![scalar.png](https://miro.medium.com/max/1400/1*X0mQTqfgj06XV8jkHg6Fbg.png)

## Vector Clocks
- Causal consistency happens

![vector.png](https://miro.medium.com/max/1064/1*xvQm1wP0v0eSmp3pnuIbEA.png)

## NB
Also need to think how to visualize this stuff. Maybe interactive console?