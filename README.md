here be ~~dragons~~ examples to implement Logical Clock from this [magical book](https://www.amazon.com/Distributed-Systems-Maarten-van-Steen/dp/1543057381)

I use RX.NET to mimic a distributed system.

Code's ugly, welp, but what else is new

### Logical Clock
#### Model
 - Two processes, one streams 'even' events, one 'uneven'.
 - Each process have a 50% chance to publish an event to his only friend

 - Proving that eventual consistency happens (clocks are synced at the end)
 - Proving no causal consistency will

### Vector Clocks
TBD