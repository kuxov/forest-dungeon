INCLUDE Globals.ink

{ mouse == "0": -> main | -> hey }

=== main ===
Ты {name}, о котором все говорят?
Я думала ты выше
Всё равно спасибо тебе! Я сама вернусь в деревню
...
Скажи, а ты уже нашел енота?
{ raccoon == "0" : -> save_him | -> saved_him }

=== save_him ===
Он должен быть где-то в лесу, если он еще жив...
Скажи, ты спасешь его?
    + [Да]
        А, хорошо...
        -> bye
    + [Нет]
        Отлично, он мне никогда не нравился! 
        -> bye
        
=== saved_him ===
Он уже вернулся в деревню?
Ура...
-> bye

=== bye ===
Ладно, пока
~ mouse = "1"
-> END 

=== hey ===
Спасибо тебе еще раз
-> END