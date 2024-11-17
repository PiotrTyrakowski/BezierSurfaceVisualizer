# Instrukcja Użytkowania

## Przegląd Kontrolek

### Gęstość Siatki (Gęstość siatki):
- Reguluj liczbę podziałów w siatce, co wpływa na gładkość powierzchni.

### Kąty Rotacji (Kąt alfa i Kąt beta):
- Obracaj powierzchnię wokół osi X (alfa) i Y (beta).

### Parametry Oświetlenia (Współczynnik kd, Współczynnik ks, Współczynnik m):
- **Kd**: Współczynnik odbicia dyfuzyjnego.
- **Ks**: Współczynnik odbicia spekularnego.
- **M**: Współczynnik połysku określający wielkość refleksu spekularnego.

### Modyfikuj Mape Normali (Modyfikuj mape norm):
- Przełączanie zastosowania map normalnych dla lepszych efektów oświetleniowych.

### Opcje Renderowania:
- **Rysuj krawędzie (Wireframe)**: Pokaż lub ukryj krawędzie siatki.
- **Stały kolor (Solid Color)**: Renderuj powierzchnię jednolitym kolorem.
- **Tekstura (Texture)**: Nałóż teksturę na powierzchnię.
- **Brak wypełnienia (No Fill)**: Renderuj tylko wireframe bez wypełnienia powierzchni.

### Zarządzanie Kolorami i Teksturami:
- **Zmień kolor obiektu (Change Object Color)**: Otwórz wybierak kolorów, aby ustawić nowy kolor powierzchni.
- **Zmień kolor światła (Change Light Color)**: Otwórz wybierak kolorów, aby ustawić nowy kolor źródła światła.
- **Wczytaj teksturę (Load Texture)**: Załaduj plik obrazu jako teksturę powierzchni.
- **Wczytaj mapę normalnych (Load Normal Map)**: Załaduj mapę normalnych, aby wpłynąć na obliczenia oświetlenia.

### Poziom Z Światła (Poziom Z światła):
- Reguluj współrzędną Z źródła światła.

### Kontrolki Animacji:
- **Start animacji (Start Animation)**: Rozpocznij animację pozycji źródła światła.
- **Stop animacji (Stop Animation)**: Zatrzymaj animację źródła światła.

---

## Kroki do Wizualizacji i Manipulacji Powierzchnią Béziera

### Ładuj Punkty Kontrolne:
1. Upewnij się, że plik `control_points.txt` jest poprawnie sformatowany i znajduje się w katalogu aplikacji.
2. Aplikacja automatycznie ładuje te punkty podczas inicjalizacji.

### Reguluj Gęstość Podziału:
- Użyj suwaka **Gęstość siatki**, aby ustawić rozdzielczość siatki. Wyższe wartości skutkują gładszymi powierzchniami.

### Rotuj Powierzchnię:
- Zmień **Kąt alfa** i **Kąt beta**, aby obrócić powierzchnię wokół osi X i Y odpowiednio.

### Konfiguruj Oświetlenie:
- Reguluj **Kd**, **Ks** i **M**, aby zmienić sposób, w jaki powierzchnia współdziała ze światłem.
- Użyj suwaka **Poziom Z światła**, aby przesunąć źródło światła wzdłuż osi Z.

### Nakładaj Tekstury i Mapy Normali:
- Kliknij **Wczytaj teksturę**, aby nałożyć obraz tekstury na powierzchnię.
- Kliknij **Wczytaj mapę normalnych**, aby nałożyć mapę normalnych dla lepszych szczegółów oświetlenia.

### Przełącz Opcje Renderowania:
- Użyj przycisków radiowych, aby przełączyć między renderowaniem jednolitym kolorem, teksturowanym lub tylko wireframe.
- Zaznacz lub odznacz **Rysuj krawędzie**, aby pokazać lub ukryć krawędzie siatki.

### Personalizuj Kolory:
- Kliknij **Zmień kolor obiektu**, aby wybrać nowy kolor dla powierzchni.
- Kliknij **Zmień kolor światła**, aby wybrać nowy kolor dla źródła światła.

### Animuj Źródło Światła:
- Kliknij **Start animacji**, aby rozpocząć rotację źródła światła wokół powierzchni.
- Kliknij **Stop animacji**, aby zatrzymać animację.
