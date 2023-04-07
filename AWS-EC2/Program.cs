using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.Runtime.Internal;
using AWS_EC2;
using System.Net.Sockets;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var myAws = new MyAWS();

        #region CreateAndDescribeSecurityGroup
        await myAws.CreateSecurityGroup("http80", "http80-desc");

        var secGroups = await myAws.DescribeSecurityGroups();

        var firstSecGroup = secGroups.SecurityGroups.FirstOrDefault(x => x.GroupName.Equals("http80"));
        
        //we can chack this rule in ec2->securityrules->http80->inboundrules
        Console.WriteLine(await myAws.AuthorizeSecurityGroupIngress(firstSecGroup.GroupName));

        Console.WriteLine("Security groups: ");
        foreach (var secGroup in secGroups.SecurityGroups)
        {
            Console.WriteLine($"{secGroup.OwnerId} - {secGroup.GroupName} - {secGroup.Description}");
        }
        #endregion

        //#region AllInstances
        //Console.WriteLine("instances of EC2:");
        //var instances = await myAws.DescribeInstances();
        //instances.ForEach(instance =>
        //{
        //    Console.Write($"Instance ID: {instance.InstanceId}");
        //});
        //#endregion

        //var firstInstance = await myAws.DescribeInstanceById(instances[0].InstanceId);

        //#region GetInstanceById
        //Console.WriteLine($"{firstInstance.InstanceId} state: {firstInstance.State.Name}");
        //#endregion

        //#region StartInstance
        //Console.WriteLine($"starting instance {firstInstance.InstanceId}");
        //await myAws.StartInstance(firstInstance.InstanceId);
        //#endregion

        //#region StopInstance
        //Console.WriteLine($"stopping instance {firstInstance.InstanceId}");
        //await myAws.StopInstance(firstInstance.InstanceId);
        //#endregion
    }
}