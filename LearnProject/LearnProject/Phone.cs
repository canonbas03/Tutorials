namespace LearnProject
{
    public class Phone
    {
        public string Brand { get; set; }
        public string Model { get; set; }
        public int BatteryLevel { get; set; }

        public Phone(string brand, string model)
        {
            Brand = brand;
            Model = model;
            BatteryLevel = 100;
        }
        public Phone(string brand, string model, int batteryLevel)
        {
            Brand = brand;
            Model = model;
            BatteryLevel = Math.Min(batteryLevel,100);
        }


        public void ShowInfo()
        {
            Console.WriteLine($"{Brand} {Model} - Battery: {BatteryLevel}%");
        }

        public void UseBattery(int used)
        {
            if(BatteryLevel >= used)
            {
                BatteryLevel -= used;
            }
        }
        
        public void ChargeBattery(int charge)
        {
            BatteryLevel += charge;
            if(BatteryLevel > 100)
            {
                BatteryLevel = 100;
            }
        }

        public void UsePhone(int minutes)
        {
            int totalEnergy = minutes * 2;
            if ((BatteryLevel - totalEnergy) >= 0)
            {
                BatteryLevel -= totalEnergy;
            }
            else
            {
                Console.WriteLine("Battery too low");
            }
        }

        public void DrainBattery()
        {
            BatteryLevel = 0;
            Console.WriteLine("Phone is now off");
        }

        public void PhoneUsage()
        {
            Random random = new Random();
            int drain = random.Next(0, 100);
            UseBattery(drain);
            Console.WriteLine($"Battery left: {BatteryLevel}%");
        }
    }
}