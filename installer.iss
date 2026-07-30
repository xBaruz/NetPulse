#define MyAppName "NetPulse Log Monitor"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "xBaruz"
#define MyAppExeName "NetPulse.Worker.exe"
#define MyAppServiceName "NetPulseService"

[Setup]
AppId={{3CBACF19-A749-438E-99E2-7A7827523193}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\{#MyAppName}
UninstallDisplayIcon={app}\{#MyAppExeName}

ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible

DefaultGroupName={#MyAppName}
OutputDir=E:\projekty\NetPulse
OutputBaseFilename=NetPulse_Setup_v1.0.0
SolidCompression=yes
WizardStyle=modern dynamic
PrivilegesRequired=admin

[Languages]
Name: "polish"; MessagesFile: "compiler:Languages\Polish.isl"
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "E:\projekty\NetPulse\NetPulse.Worker\bin\Publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Run]
; 1. Rejestracja usługi w systemie Windows (bez startowania od razu!)
Filename: "{sys}\sc.exe"; Parameters: "create ""{#MyAppServiceName}"" binPath= ""{app}\{#MyAppExeName}"" start= auto displayname= ""{#MyAppName}"""; Flags: runhidden

[UninstallRun]
Filename: "{sys}\sc.exe"; Parameters: "stop ""{#MyAppServiceName}"""; Flags: runhidden
Filename: "{sys}\sc.exe"; Parameters: "delete ""{#MyAppServiceName}"""; Flags: runhidden

[Code]
var
  TelegramPage: TInputQueryWizardPage;
  HelpLabel: TNewStaticText;

// Funkcja pomocnicza do usuwania WSZYSTKICH spacji z ciągu znaków
function CleanInput(Value: String): String;
var
  Cleaned: String;
begin
  Cleaned := Trim(Value);
  StringChange(Cleaned, ' ', ''); // Usuwa ewentualne spacje wewnątrz tekstu
  Result := Cleaned;
end;

procedure InitializeWizard();
begin
  TelegramPage := CreateInputQueryPage(
    wpSelectDir,
    'Konfiguracja Telegrama',
    'Gdzie wysyłać alerty o błędach?',
    'Podaj Bot Token oraz Chat ID. Poniżej znajdziesz instrukcję, jak je uzyskać.'
  );

  TelegramPage.Add('Bot Token:', False);
  TelegramPage.Add('Chat ID:', False);

  HelpLabel := TNewStaticText.Create(TelegramPage);
  HelpLabel.Parent := TelegramPage.Surface;
  HelpLabel.Top := TelegramPage.Edits[1].Top + TelegramPage.Edits[1].Height + 15;
  HelpLabel.Left := 0;
  HelpLabel.Width := TelegramPage.SurfaceWidth;
  HelpLabel.Height := 120;
  HelpLabel.WordWrap := True;
  HelpLabel.Caption :=
    'Jak uzyskać potrzebne dane?' + #13#10 +
    '1. BOT TOKEN: Otwórz Telegram, znajdź bota @BotFather i wyślij komendę /newbot. Zapisz wygenerowany Token.' + #13#10 +
    '2. Uruchom bota: Znajdź swojego nowego bota na Telegramie i kliknij "START".' + #13#10 +
    '3. CHAT ID: Znajdź bota @userinfobot i wyślij mu dowolną wiadomość – odpisze podając Twój numer Id.';
end;

procedure CurStepChanged(CurStep: TSetupStep);
var
  ConfigFilePath: String;
  JsonContent: String;
  BotTokenValue: String;
  ChatIdValue: String;
  ResultCode: Integer;
begin
  if CurStep = ssPostInstall then
  begin
    // Oczyszczamy wpisane wartości ze spacji!
    BotTokenValue := CleanInput(TelegramPage.Values[0]);
    ChatIdValue := CleanInput(TelegramPage.Values[1]);

    ConfigFilePath := ExpandConstant('{app}\appsettings.json');

    JsonContent := 
      '{' + #13#10 +
      '  "Logging": {' + #13#10 +
      '    "LogLevel": {' + #13#10 +
      '      "Default": "Information",' + #13#10 +
      '      "Microsoft.Hosting.Lifetime": "Information"' + #13#10 +
      '    }' + #13#10 +
      '  },' + #13#10 +
      '  "Watcher": {' + #13#10 +
      '    "DirectoryPath": "C:\\Logs",' + #13#10 +
      '    "Filter": "*.log"' + #13#10 +
      '  },' + #13#10 +
      '  "Telegram": {' + #13#10 +
      '    "BotToken": "' + BotTokenValue + '",' + #13#10 +
      '    "ChatId": "' + ChatIdValue + '"' + #13#10 +
      '  },' + #13#10 +
      '  "ConnectionStrings": {' + #13#10 +
      '    "DefaultConnection": "Data Source=netpulse.db"' + #13#10 +
      '  }' + #13#10 +
      '}';

    // 1. Zapisujemy wyczyszczony appsettings.json
    SaveStringToFile(ConfigFilePath, JsonContent, False);

    // 2. Uruchamiamy usługę
    Exec(ExpandConstant('{sys}\sc.exe'), 'start "NetPulseService"', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
  end;
end;