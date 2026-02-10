export enum TRStatus {
  Ok = "OK",
  ExistEIK = "existEIK",
  MissingEIK = "missingEIK"
}

export enum TRMessage {
  ExistEIK = "Институция с този Булстат/ЕИК вече е вписана в Регистъра. За преобразуване или закриване, моля стартирайте процедура от раздел 'Действащи институции'.",
  MissingEIK = "Не са намерени данни по посочения БУЛСТАТ/ЕИК."
}
