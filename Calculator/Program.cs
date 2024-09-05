using System;
class Program{
    static int Precedence(char op){
        switch(op){
            case '(':case ')':
                return 0;
            case '+':case '-':
                return 1;
            case '*':case '/':
                return 2;
            case '^':
                return 3;
            default:
                throw new ArgumentException($"{op}は無効な演算子です");
        }
    }
    static Queue<string> ConvertToRPN(string str){
        Queue<string> queue = new Queue<string>();
        Stack<char> stack = new Stack<char>();

        string temp;
        int i = 0;
        while(i < str.Length){
            if(Char.IsWhiteSpace(str[i])) continue;

            switch(str[i]){
                case char c when Char.IsDigit(c):
                    temp = "";
                    Number:
                    while(i < str.Length && ( Char.IsDigit(str[i]) || str[i] == '.')){
                        temp += str[i];
                        i++;
                    }
                    queue.Enqueue(temp);
                    break;
                case '(':
                    stack.Push(str[i]);
                    i++;
                    break;
                case ')':
                    while(stack.Count > 0 && stack.Peek() != '('){
                        queue.Enqueue(stack.Pop().ToString());
                    }
                    stack.Pop();
                    i++;
                    break;
                default:
                    try{
                        if(queue.Count == 0){
                            if(stack.Count == 0){
                                if(str[i] == '-'){
                                    temp = "-";
                                    i++;
                                    goto Number;
                                }
                                else if(str[i] == '+'){
                                    continue;
                                }
                            }
                        }
                        while(stack.Count > 0 && Precedence(stack.Peek()) >= Precedence(str[i])){
                            queue.Enqueue(stack.Pop().ToString());
                        }
                        stack.Push(str[i]);
                        i++;
                        break;
                    }
                    catch(ArgumentException e){
                        Console.WriteLine("{0}: {1}",e.GetType().Name,e.Message);
                        throw new ArgumentException();
                    }
            }
        }
        while(stack.Count > 0){
            queue.Enqueue(stack.Pop().ToString());
        }
        return queue;
    }

    static void Calculate(Queue<string> queue, out int result){
        Stack<int> stack = new Stack<int>();
        var i = 0;
        int itemp , pop1, pop2;
        string stemp;
        while(i < queue.Count){
            stemp = queue.Dequeue();
            if(int.TryParse(stemp,out itemp)){
                stack.Push(itemp);
            }
            else{
                pop1 = stack.Pop();
                pop2 = stack.Pop();
                switch(stemp){
                    case "+":
                        stack.Push(pop2 + pop1);
                        break;
                    case "-":
                        stack.Push(pop2 - pop1);
                        break;
                    case "*":
                        stack.Push(pop2 * pop1);
                        break;
                    case "/"://小数となるとき
                        stack.Push(pop2 / pop1);
                        break;
                    case "^":
                        stack.Push(Pow(pop2,pop1));
                        break;
                }
            }
        }
        result = stack.Pop();
        return;
    }
    static int Pow(int a, int b){
        int result = 1;
        for(var i = 0; i < b; i++){
            result *= a;
        }
        return result;
    }

    static void Main(){

        Console.Write("計算式を入力してください ");
        string input = Console.ReadLine()!.Trim();
        bool IsDouble = input.Contains('.');

        try{
            var converted = ConvertToRPN(input);
            Console.WriteLine(String.Join(" ", converted));
            // if(IsDouble) double result = 
            if(!IsDouble) Calculate(converted,out int result);
            // Console.WriteLine(result);
            return;
        }
        catch(ArgumentException){
            return;
        }
        catch{
            Console.WriteLine("予期しない例外が発生しました");
            return;
        }
    }
}