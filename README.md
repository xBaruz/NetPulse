# NetPulse Log Monitor

NetPulse Log Monitor to usługa systemowa dla systemu Windows (Windows Service) napisana w języku C# (.NET 10), przeznaczona do automatycznego i pasywnego monitorowania plików logów w czasie rzeczywistym. 

Aplikacja obserwuje wskazany katalog, wykrywa nowe wpisy o błędach i natychmiast wysyła alerty na kanał Telegram za pośrednictwem Telegram Bot API. Wszystkie zdarzenia są również zapisywane lokalnie w bazie danych SQLite.

---

## Główne Funkcjonalności

- Usługa Windows Service: Działa w tle jako usługa systemowa z automatycznym startem przy uruchomieniu systemu, bez konieczności logowania użytkownika.
- Real-time Log Watcher: Wykorzystuje mechanizm FileSystemWatcher do natychmiastowego wykrywania nowych wpisów w plikach tekstowych (np. app.log).
- Integracja z Telegram API: Błyskawiczna wysyłka powiadomień ze szczegółami błędów na wskazany czat/grupę Telegrama.
- Lokalna Retencja Danych: Równoległy zapis wyłapanych błędów do lokalnej bazy danych SQLite (netpulse.db).
- Dedykowany Instalator: Gotowy plik instalacyjny (.exe) automatycznie konfiguruje, rejestruje i uruchamia usługę w systemie.

---

## Architektura Projektu

Projekt został zaprojektowany zgodnie z zasadami Clean Architecture:

- NetPulse.Domain: Encje domenowe, interfejsy oraz reguły biznesowe.
- NetPulse.Application: Dedykowane usługi aplikacyjne, logika przetwarzania logów i wysyłki alertów.
- NetPulse.Infrastructure: Baza danych (Entity Framework Core / SQLite), obsługa FileSystemWatcher oraz integracja z API Telegrama.
- NetPulse.Worker: Główny punkt wejścia (BackgroundService / Windows Service) zarządzający cyklem życia aplikacji.
- NetPulse.Cli: Opcjonalny interfejs wiersza poleceń do podglądu zarchiwizowanych logów z bazy danych.

---

## Wymagania Wdrożeniowe

Dzięki opcji publikacji Self-Contained, użytkownik docelowy nie musi posiadać zainstalowanego środowiska .NET SDK.

- System operacyjny: Windows 10 / Windows 11 / Windows Server (64-bit)
- Uprawnienia: Administratora (wymagane do rejestracji usługi systemowej podczas instalacji)

---

## Szybka Instalacja (Dla Użytkowników)

1. Przejdź do sekcji Releases po prawej stronie repozytorium na GitHubie i pobierz najnowszą wersję pliku `NetPulse_Setup_v1.0.0.exe`.
2. Uruchom pobrany plik jako Administrator.
3. W oknie instalatora podaj swoje dane dostępowe:
   - Bot Token: Token uzyskany od bota @BotFather.
   - Chat ID: Twój identyfikator uzyskany np. od bota @userinfobot.
4. Kliknij Zainstaluj – instalator sam skonfiguruje plik `appsettings.json`, zarejestruje usługę `NetPulseService` i uruchomi ją w tle.

---

## Domyślne Ścieżki w Systemie

- Monitorowany folder: `C:\Logs` (konfigurowalny w `appsettings.json`)
- Filtrowane pliki: `*.log`
- Plik bazy danych: `C:\Program Files\NetPulse Log Monitor\netpulse.db`
- Plik konfiguracyjny: `C:\Program Files\NetPulse Log Monitor\appsettings.json`

---

## Testowanie Działania

Aby sprawdzić, czy usługa poprawnie przechwytuje błędy i wysyła alert na Telegram, wykonaj polecenie w konsoli PowerShell:

Add-Content -Path "C:\Logs\app.log" -Value "ERROR: Testowy blad systemu wygenerowany z PowerShell"

Powiadomienie z informacją o błędzie powinno natychmiast pojawić się na Twoim koncie Telegram.

---

## Instrukcja dla Deweloperów (Budowanie ze Źródeł)

Jeśli chcesz samodzielnie zmodyfikować kod i skompilować projekt:

1. Sklonuj repozytorium:
   git clone https://github.com/xBaruz/NetPulse.git
   cd NetPulse

2. Zbuduj wersję produkcyjną aplikacji Worker:
   dotnet publish NetPulse.Worker/NetPulse.Worker.csproj -c Release -r win-x64 --self-contained true -o NetPulse.Worker/bin/Publish

3. (Opcjonalnie) Otwórz plik `installer.iss` w programie Inno Setup i skompiluj własny instalator (`Ctrl + F9`).