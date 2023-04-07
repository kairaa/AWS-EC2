using Amazon.EC2;
using Amazon.EC2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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

        #region SecurityGroup
        public async Task<DescribeSecurityGroupsResponse> DescribeSecurityGroups()
        {
            var request = new DescribeSecurityGroupsRequest
            {
                MaxResults = 10,
            };

            var response = await eC2Client.DescribeSecurityGroupsAsync(request);

            return response;
        }

        public async Task<string> CreateSecurityGroup(string groupName, string groupDescription)
        {
            var response = await eC2Client.CreateSecurityGroupAsync(
                new CreateSecurityGroupRequest(groupName, groupDescription));

            return response.GroupId;
        }

        public async Task<bool> AuthorizeSecurityGroupIngress(string groupName)
        {
            // Get the IP address for the local computer.
            var ipAddress = await GetIpAddress();
            Console.WriteLine($"Your IP address is: {ipAddress}");
            var ipRanges = new List<IpRange> { new IpRange { CidrIp = $"{ipAddress}/32" } };
            var permission = new IpPermission
            {
                Ipv4Ranges = ipRanges,
                IpProtocol = "tcp",
                FromPort = 22,
                ToPort = 22
            };
            var permissions = new List<IpPermission> { permission };
            var response = await eC2Client.AuthorizeSecurityGroupIngressAsync(
                new AuthorizeSecurityGroupIngressRequest(groupName, permissions));
            return response.HttpStatusCode == HttpStatusCode.OK;
        }
        #endregion

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

        public async Task<List<InstanceStateChange>> TerminateInstances(string ec2InstanceId)
        {
            var request = new TerminateInstancesRequest
            {
                InstanceIds = new List<string> { ec2InstanceId }
            };

            var response = await eC2Client.TerminateInstancesAsync(request);
            return response.TerminatingInstances;
        }

        private static async Task<string> GetIpAddress()
        {
            var httpClient = new HttpClient();
            var ipString = await httpClient.GetStringAsync("https://checkip.amazonaws.com");

            // The IP address is returned with a new line
            // character on the end. Trim off the whitespace and
            // return the value to the caller.
            return ipString.Trim();
        }

    }
}
