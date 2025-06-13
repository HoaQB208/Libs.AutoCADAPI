; Script for Inno Setup. Download Inno Setup at https://jrsoftware.org/isdl.php

[Setup]
AppName=Libs.AutoCADAPI
AppVersion=0.0.1
OutputBaseFilename=Libs.AutoCADAPI Installer v0.0.1
DefaultDirName={commonappdata}\Autodesk\ApplicationPlugins\Libs.AutoCADAPI.bundle
PrivilegesRequired=admin
DisableStartupPrompt=yes
DisableWelcomePage=yes
DisableDirPage=yes
DisableReadyPage=yes
DisableProgramGroupPage=yes
Compression=lzma
SolidCompression=yes
Uninstallable=yes
CreateUninstallRegKey=yes
UninstallDisplayIcon=C:\Windows\System32\shell32.dll,162
UninstallDisplayName=Libs.AutoCADAPI AutoCAD Add-in
ArchitecturesInstallIn64BitMode=x64compatible

[Files]
Source: "D:\PROJECTS\Libs\Libs.AutoCADAPI\Libs.AutoCADAPI\bin\Libs.AutoCADAPI.bundle\*"; DestDir: "{app}"; Flags: recursesubdirs createallsubdirs ignoreversion

[Icons]

[Run]

[UninstallDelete]
Type: filesandordirs; Name: "{app}"

[Code]
function InitializeSetup(): Boolean;
var
  oldDir: String;
  success: Boolean;
begin
  oldDir := ExpandConstant('{commonappdata}\Autodesk\ApplicationPlugins\Libs.AutoCADAPI.bundle');
  if DirExists(oldDir) then
  begin
    success := DelTree(oldDir, True, True, True);
    if not success then
    begin
      MsgBox('Unable to uninstall the previous version.' + #13#10 + 
            'AutoCAD may be running and using the Libs.AutoCADAPI add-in.' + #13#10 +
            'Please close all instances of AutoCAD before continuing the installation.' + #13#10 +
            '' + #13#10 +
            'Không thể gỡ phiên bản cũ.' + #13#10 +
            'Có thể AutoCAD đang chạy và đang sử dụng add-in Libs.AutoCADAPI.' + #13#10 +
            'Vui lòng tắt tất cả các phiên bản AutoCAD trước khi tiếp tục cài đặt.',
            mbError, MB_OK);
      Result := False;
      Exit;
    end;
  end;
  Result := True;
end;