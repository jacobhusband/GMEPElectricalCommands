(vl-load-com)

; Define the S::STARTUP function to load the DLL at startup
(defun S::STARTUP ()
  (command "NETLOAD" "C:\\Users\\jakeh\\source\\repos\\GMEPElectricalCommands\\GMEPElectricalCommands\\bin\\Debug\\ElectricalCommands.dll")
  (princ "ElectricalCommands.dll loaded at startup.\n")
)

; Define a custom command to load the DLL on demand
(defun c:LoadMyDLL ()
  (command "NETLOAD" "C:\\Users\\jakeh\\source\\repos\\GMEPElectricalCommands\\GMEPElectricalCommands\\bin\\Debug\\ElectricalCommands.dll")
  (princ "ElectricalCommands.dll loaded on demand.\n")
)

(princ "Type 'LoadMyDLL' to load the DLL on demand.\n")
(princ)
