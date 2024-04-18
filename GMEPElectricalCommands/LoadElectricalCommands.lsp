(vl-load-com)

(defun S::STARTUP ()
  (command "NETLOAD" "C:\\Users\\jakeh\\source\\repos\\GMEPElectricalCommands\\GMEPElectricalCommands\\bin\\Debug\\ElectricalCommands.dll")
  (princ "ElectricalCommands.dll loaded.")
)

(princ)