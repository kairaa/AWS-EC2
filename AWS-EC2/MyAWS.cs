using Amazon.EC2;
using Amazon.EC2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWS_EC2
{
    public class MyAWS
    {
        public AmazonEC2Client eC2Client { get; set; }

        public MyAWS()
        {
            eC2Client = new AmazonEC2Client();
        }

        public async Task<DescribeSecurityGroupsResponse> DescribeSecurityGroups(AmazonEC2Client ec2Client)
        {
            var request = new DescribeSecurityGroupsRequest
            {
                MaxResults = 10,
            };

            var response = await ec2Client.DescribeSecurityGroupsAsync(request);

            return response;
        }

        public async Task<List<Instance>> DescribeInstances()
        {
            var instances = new List<Instance>();
            var paginator = eC2Client.Paginators.DescribeInstances(new DescribeInstancesRequest());

            await foreach (var response in paginator.Responses)
            {
                foreach (var reservation in response.Reservations)
                {
                    foreach (var instance in reservation.Instances)
                    {
                        instances.Add(instance);
                    }
                }
            }
            return instances;
        }

        public async Task<Instance> DescribeInstanceById(string instanceId)
        {
            var instances = await DescribeInstances();
            var instance = instances.SingleOrDefault(x => x.InstanceId == instanceId);
            //Console.WriteLine($"State: {instance.State}");
            //Console.WriteLine($"State: {instance.State}");
            return instance;
        }

        public async Task StartInstance(string ec2InstanceId)
        {
            var request = new StartInstancesRequest
            {
                InstanceIds = new List<string> { ec2InstanceId },
            };

            var response = await eC2Client.StartInstancesAsync(request);

            if (response.StartingInstances.Count > 0)
            {
                var instances = response.StartingInstances;
                instances.ForEach(i =>
                {
                    Console.WriteLine($"Successfully started the EC2 instance with instance ID: {i.InstanceId}.");
                });
            }
        }

        public async Task StopInstance(string ec2InstanceId)
        {
            var request = new StopInstancesRequest
            {
                InstanceIds = new List<string> { ec2InstanceId },
            };

            var response = await eC2Client.StopInstancesAsync(request);

            if (response.StoppingInstances.Count > 0)
            {
                var instances = response.StoppingInstances;
                instances.ForEach(i =>
                {
                    Console.WriteLine($"Successfully stopped the EC2 Instance " +
                                      $"with InstanceID: {i.InstanceId}.");
                });
            }
        }
    }
}
