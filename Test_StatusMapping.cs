using System;
using System.Collections.Generic;
using pet_spa_system1.Helpers;

namespace pet_spa_system1.Tests
{
    /// <summary>
    /// Test script để verify StatusMappingHelper logic
    /// </summary>
    public class StatusMappingTest
    {
        public static void RunTests()
        {
            Console.WriteLine("=== TESTING STATUS MAPPING HELPER ===");
            Console.WriteLine();

            // Test case từ vấn đề của bạn
            TestCaseFromUserIssue();

            // Test case từ câu hỏi mới
            TestMixedCompletedCancelled();

            // Test các trường hợp khác
            TestAllCombinations();
            
            Console.WriteLine("=== TESTS COMPLETED ===");
        }

        /// <summary>
        /// Test case từ vấn đề thực tế của user
        /// Cả 2 services đều Cancelled (4) -> Appointment phải là Cancelled (5)
        /// </summary>
        private static void TestCaseFromUserIssue()
        {
            Console.WriteLine("🔍 TEST CASE: User's Issue - Both services cancelled");
            
            var serviceStatuses = new List<int?> { 4, 4 }; // Cả 2 đều Cancelled
            var expectedAppointmentStatus = StatusMappingHelper.AppointmentStatus.Cancelled; // 5
            
            var actualAppointmentStatus = StatusMappingHelper.CalculateAppointmentStatusFromServices(serviceStatuses);
            
            Console.WriteLine($"Input: Services = [{string.Join(", ", serviceStatuses)}]");
            Console.WriteLine($"Expected: Appointment Status = {expectedAppointmentStatus} ({StatusMappingHelper.GetAppointmentStatusName(expectedAppointmentStatus)})");
            Console.WriteLine($"Actual: Appointment Status = {actualAppointmentStatus} ({StatusMappingHelper.GetAppointmentStatusName(actualAppointmentStatus)})");
            Console.WriteLine($"Result: {(actualAppointmentStatus == expectedAppointmentStatus ? "✅ PASS" : "❌ FAIL")}");
            Console.WriteLine();
        }

        /// <summary>
        /// Test case: 1 service Completed + 1 service Cancelled
        /// </summary>
        private static void TestMixedCompletedCancelled()
        {
            Console.WriteLine("🔍 TEST CASE: Mixed Completed + Cancelled");

            var serviceStatuses = new List<int?> { 3, 4 }; // 1 Completed + 1 Cancelled
            var expectedAppointmentStatus = StatusMappingHelper.AppointmentStatus.Completed; // 4

            var actualAppointmentStatus = StatusMappingHelper.CalculateAppointmentStatusFromServices(serviceStatuses);

            Console.WriteLine($"Input: Services = [{string.Join(", ", serviceStatuses)}] (Completed + Cancelled)");
            Console.WriteLine($"Expected: Appointment Status = {expectedAppointmentStatus} ({StatusMappingHelper.GetAppointmentStatusName(expectedAppointmentStatus)})");
            Console.WriteLine($"Actual: Appointment Status = {actualAppointmentStatus} ({StatusMappingHelper.GetAppointmentStatusName(actualAppointmentStatus)})");
            Console.WriteLine($"Logic: Có ít nhất 1 service hoàn thành → Appointment = Completed");
            Console.WriteLine($"Result: {(actualAppointmentStatus == expectedAppointmentStatus ? "✅ PASS" : "❌ FAIL")}");
            Console.WriteLine();
        }

        /// <summary>
        /// Test tất cả các combination
        /// </summary>
        private static void TestAllCombinations()
        {
            Console.WriteLine("🔍 TEST ALL COMBINATIONS:");
            Console.WriteLine();

            var testCases = new[]
            {
                // All same status
                new { Services = new int?[] { 1, 1 }, Expected = 1, Description = "All Pending" },
                new { Services = new int?[] { 2, 2 }, Expected = 3, Description = "All InProgress" },
                new { Services = new int?[] { 3, 3 }, Expected = 4, Description = "All Completed" },
                new { Services = new int?[] { 4, 4 }, Expected = 5, Description = "All Cancelled" },
                
                // Mixed status
                new { Services = new int?[] { 1, 2 }, Expected = 3, Description = "Pending + InProgress" },
                new { Services = new int?[] { 1, 3 }, Expected = 2, Description = "Pending + Completed" },
                new { Services = new int?[] { 1, 4 }, Expected = 2, Description = "Pending + Cancelled" },
                new { Services = new int?[] { 2, 3 }, Expected = 3, Description = "InProgress + Completed" },
                new { Services = new int?[] { 2, 4 }, Expected = 3, Description = "InProgress + Cancelled" },
                new { Services = new int?[] { 3, 4 }, Expected = 4, Description = "Completed + Cancelled (Mixed case)" },
                
                // Three services
                new { Services = new int?[] { 1, 2, 3 }, Expected = 3, Description = "Pending + InProgress + Completed" },
                new { Services = new int?[] { 1, 2, 4 }, Expected = 3, Description = "Pending + InProgress + Cancelled" },
                new { Services = new int?[] { 1, 3, 4 }, Expected = 2, Description = "Pending + Completed + Cancelled" },
                new { Services = new int?[] { 2, 3, 4 }, Expected = 3, Description = "InProgress + Completed + Cancelled" },
                
                // Edge cases
                new { Services = new int?[] { }, Expected = 1, Description = "No services" },
                new { Services = new int?[] { null }, Expected = 1, Description = "Null service" },
                new { Services = new int?[] { 1, null }, Expected = 2, Description = "Pending + Null" },
            };

            foreach (var testCase in testCases)
            {
                var actual = StatusMappingHelper.CalculateAppointmentStatusFromServices(testCase.Services);
                var result = actual == testCase.Expected ? "✅ PASS" : "❌ FAIL";
                
                Console.WriteLine($"{testCase.Description}:");
                Console.WriteLine($"  Services: [{string.Join(", ", testCase.Services.Select(s => s?.ToString() ?? "null"))}]");
                Console.WriteLine($"  Expected: {testCase.Expected} ({StatusMappingHelper.GetAppointmentStatusName(testCase.Expected)})");
                Console.WriteLine($"  Actual: {actual} ({StatusMappingHelper.GetAppointmentStatusName(actual)})");
                Console.WriteLine($"  Result: {result}");
                Console.WriteLine();
            }
        }
    }
}

// Uncomment để chạy test
// StatusMappingTest.RunTests();
