# Permenence

I used the following guides
Original guide: https://aloiskraus.wordpress.com/2017/04/23/the-definitive-serialization-performance-guide/
.NET 4.7 update guide: https://aloiskraus.wordpress.com/2018/05/06/serialization-performance-update-with-net-4-7-2/

I first tried using BinaryFormatter serialization but it was catastrophically
slow. Then I tried protobuf-net but unity didn't recognize the nuget package
for some reason and I could not find enough information online to fix the issue.
Lastly I settled on MessagePackSharp
(https://github.com/neuecc/MessagePack-CSharp). There are four things to keep
in mind with MessagePackSharp. The first is that it is a tree type serializer as
opposed to a graph type meaning it cannot serialize cyclic references. Second is
that in order to instantiate an object while deserializing it requires a
constructor (optionally marked with [SerializationConstructor], I choose to
always mark). In most cases in this project classes contain two constructors,
the one used when intitially creating the object and the one used when
deserializing. Third it can only serialize public members. Lastly, it can't 
serialize tuples.

Also, I've made the questionable decision the persist terrain type, which James
has pointed out is uneccesary since you can just generate the terrain again.
Load time is short enough atm that I'm not going to worry about it but I'll
keep it in mind.