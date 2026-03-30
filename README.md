Projekt przedstawia prosty system wypożyczania sprzętu przez użytkowników. 
System pozwala na:
•	dodawanie sprzętu i użytkowników,
•	wypożyczanie i zwracanie sprzętu,
•	naliczanie kar za oddawanie sprzętu po terminie,
•	generowanie raportu podsumowującego informacje o systemie,
•	kontrolę wypożyczeń,
•	śledzenie dostępności sprzętu.

Modele domenowe
•	Equipment 
•	User 
•	Rental
Każda z tych klas odpowiada za przechowywanie danych.

Zarządzanie
•	UserService zarządza użytkownikami
•	EquipmentService zarządza sprzętem
•	RentalService obsługuje wypożyczenia
•	ReportService generuje raporty
•	LateFeePolicy nalicza kary

Interfejsy
Dla serwisów zdefiniowano interfejsy co:
•	umożliwia łatwą podmianę implementacji,
•	zmniejsza zależności między klasami.

Kohezja (spójność)
W projekcie każda klasa ma jedną odpowiedzialność:
•	EquipmentService zajmuje się wyłącznie sprzętem,
•	UserService wyłącznie użytkownikami,
•	RentalService wyłącznie procesem wypożyczania i zwrotów,
•	LateFeePolicy wyłącznie naliczaniem kar,
•	ReportService wyłącznie raportowaniem.
Dzięki temu klasy są małe i czytelne.

Odpowiedzialności klas
Każda klasa ma jedną odpowiedzialność:
•	Equipment odpowiedzialny jest za sprzęt,
•	User odpowiedzialny jest za użytkownika,
•	Rental odpowiedzialny jest za wypożyczenie,
•	Serwisy wykonują operacje na danych,
•	Polityka kar oblicza opłaty.
Taki podział jest naturalny dla systemów domenowych i ułatwia ewentualną rozbudowę.

Podział projektu wynika z chęci:
•	oddzielenia logiki biznesowej od modeli danych,
•	uproszczenia zarządzania odpowiedzialnościami,
•	zachowania czytelności i modularności.

