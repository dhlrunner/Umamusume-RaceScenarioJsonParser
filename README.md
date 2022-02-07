# Umamusume-RaceScenarioJsonParser
An C# implementation that can parse to Jobject(json) or rewrite the RaceScenario binary data of the game Umamusume.

Needs:
```
Newtonsoft.Json
```

Usage:

```csharp
RaceScenarioJsonParser raceScenario = new RaceScenarioJsonParser();
Jobject scenario = raceScenario.ParseFromBinary(bin); //Unpacked RaceScenario binary data to Jobject
Byte[] binary = raceScenario.WriteToBinary(scenario); //Convert RaceScenario Jobject to Binary;
```
