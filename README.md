NovAtelLogReader
================

## Запуск Docker-контейнера

1. Склонировать репозиторий

2. Собрать контейнер

   ```sh
   docker build -t novatellogreader .
   ```

3. Установить требуемые настройки

   Изменить настройки в файле по-умолчанию
   (`NovAtelLogReader/NovAtelLogReader/App.config`).

   Порт GPS-приёмника можно определить при помощи:

   ```sh
   ls /dev/ttyUSB*
   ```

   Либо:

   ```sh
   ls /dev/serial/by-id/*
   ```

   Так же необходимо прописать в `/etc/hosts` адрес хоста `kafka`.

4. Запустить контейнер

   Для доступа к портам GPS-приёмника необходимо пробросить эти устройства
   внутрь контейнера, примонтировав всю файловую систему */dev*:

   ```sh
   docker run --rm -v /dev/:/dev/ \
          -v /etc/hosts:/etc/hosts \
          -v `realpath NovAtelLogReader/NovAtelRunner/App.config`:/app/NovAtelRunner.dll.config \
          -v `realpath NovAtelLogReader/NovAtelLogReader/App.config`:/app/NovAtelLogReader.dll.config \
          -v `realpath NovAtelLogReader/NovAtelLogReader/NLog.config`:/app/NLog.config \
          -d novatellogreader:latest
   ```

   Либо пробросив соответвующее устройство при запуске контейнера:

   ```sh
   docker run --rm --device '<полное_имя_устройства>' \
          -v /etc/hosts:/etc/hosts \
          -v `realpath NovAtelLogReader/NovAtelRunner/App.config`:/app/NovAtelRunner.dll.config \
          -v `realpath NovAtelLogReader/NovAtelLogReader/App.config`:/app/NovAtelLogReader.dll.config \
          -v `realpath NovAtelLogReader/NovAtelLogReader/NLog.config`:/app/NLog.config \
          -d novatellogreader:latest
   ```
