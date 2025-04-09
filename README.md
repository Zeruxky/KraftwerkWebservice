This is a Simulator / Game to simply simulate the balance of producing and consuming in a powergrid.
Producing too much will push the frequency up, consuming too much will drag it down.
If the program ends up too far in either direction, it will result in a blackout and game over.

Therefore the aim is to find a good balance by playing using the endpoints
(See the swagger page for more detailed descriptions of those)

Dies ist ein Simulator / Spiel, das in einer vereinfachten Art das Ausbalancieren von Stromverbrauchern und Stromerzeugern eines Stromnetzes simuliert. Wird zu viel Strom produziert, resultiert dies in einem Anstieg der Stromfrequenz. Wird zu viel Strom konsumiert, dann reduziert sich die Stromfrequenz. Das Stromnetz operiert im normalen Betrieb um die 50 Hz. Entfernt man sich jedoch zu weit davon, dann entsteht dadurch ein Blackout.

Das Ziel dieses Spiels bzw. dieser Simulation ist es, die Netzfrequenz stabil zu halten, indem man ein ausgeglichenes Verhältnis zwischen Stromkonsumenten und Stromproduzenten aufrechterhält.

Die Stromverbraucher sind durch Bots abstrahiert, die einen zufälligen Verbrauch im Netz erzeugen. Dieser Verbrauch muss durch die Stromerzeuger, also durch dich, ausgeglichen werden. Die Erzeugung von Strom und die Information über den aktuellen Zustand des Stromnetzes erhaltet ihr über die gehostete API.