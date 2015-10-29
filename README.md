# SORACOM API no crawler

## ABOUT

SORACOMのAPI一覧情報を定期的に取得してAzureのStorage(Blob)へと書き出します。

ついでに簡単なAPIサマリも生成します。

<img width="320" alt="screen shot 2015-10-29 at 9 24 31 pm" src="https://cloud.githubusercontent.com/assets/766864/10818271/9483cdd6-7e83-11e5-8f3a-c8d227a56538.png">

Azure Web Apps内の`WebJobs`での利用を想定しています。

## TODO

- [ ] 取得したリビジョンの一覧ページを作成する
- [ ] 更新がおこなわれた際に何かしらの方法で通知する
- [ ] 各リビジョンでの更新ポイントを差分出力する

## LICENSE

MIT
