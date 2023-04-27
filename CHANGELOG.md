# Change Log

## _unreleased_

### Changed

- **settings:** Включена отсечка спутников по углу 30 градусов
  ([#4](https://github.com/mixayloff-dimaaylov/NovAtelLogReader/pull/4))

- **settings:** Включены калибровочные коэффициенты SDCB, RDBC
  ([#4](https://github.com/mixayloff-dimaaylov/NovAtelLogReader/pull/4))

- **settings:** Отключены неиспользуемые спутниковые системы SBAS, GALILEO, QZSS
  и сигналы
  ([#4](https://github.com/mixayloff-dimaaylov/NovAtelLogReader/pull/4))

- **settings:** Включены старые логи `ISMRAWTECB`, `ISMDETOBSB`, `ISMREDOBSB`
  для целей отладки
  ([#4](https://github.com/mixayloff-dimaaylov/NovAtelLogReader/pull/4))

- Теперь проект лицензируется под лицензией Apache 2.0
  ([#7](https://github.com/mixayloff-dimaaylov/NovAtelLogReader/pull/7))

### Added

- Добавлена поддержка сборки Docker-контейнера
  ([#1](https://github.com/mixayloff-dimaaylov/NovAtelLogReader/pull/1),
  [#2](https://github.com/mixayloff-dimaaylov/NovAtelLogReader/pull/2),
  [#5](https://github.com/mixayloff-dimaaylov/NovAtelLogReader/pull/5))

- Добавлены Avro-схемы логов Kafka
  ([#3](https://github.com/mixayloff-dimaaylov/NovAtelLogReader/pull/3),
  [#6](https://github.com/mixayloff-dimaaylov/NovAtelLogReader/pull/6))

- Добавлен CHANGELOG
  ([#8](https://github.com/mixayloff-dimaaylov/NovAtelLogReader/pull/8))

## [0.2.0] - 2020-08-27

### Changed

- Портирована на .NET Core для поддержки Linux

### Added

- Добавлена поддержка сигналов GLONASS
  ([`6a1fdb9`](https://github.com/mixayloff-dimaaylov/NovAtelLogReader/commit/6a1fdb9))

- Добавлено поле `LOCKTIME` в лог `RANGE`
  ([`a72e6bd`](https://github.com/mixayloff-dimaaylov/NovAtelLogReader/commit/a72e6bd))

### Fixed

- Исправлена проблема перезапуска NovAtelLogReader. Теперь нет необходимости в
  NovAtelLogRunner

### [0.1.0]

:seedling: Initial release.

- Реализована поддержка бинарных- и ASCII- логов для приёмника NovAtel OEM6 и
  отправки их в RabbitMQ и Apache Kafka

[0.2.0]: https://github.com/mixayloff-dimaaylov/NovAtelLogReader/releases/tag/0.2.0
[0.1.0]: https://github.com/mixayloff-dimaaylov/NovAtelLogReader/releases/tag/0.1.0
