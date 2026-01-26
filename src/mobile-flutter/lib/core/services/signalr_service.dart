// lib/core/services/signalr_service.dart
import 'dart:async';

import 'package:signalr_netcore/signalr_client.dart';

import '../constants/api_constants.dart';

class SignalRService {
  late HubConnection _hubConnection;
  bool _isConnected = false;
  String? _accessToken;

  final StreamController<Map<String, dynamic>> _seatChangedController =
      StreamController<Map<String, dynamic>>.broadcast();

  Stream<Map<String, dynamic>> get seatChangedStream =>
      _seatChangedController.stream;

  SignalRService({String? accessToken}) {
    _accessToken = accessToken;
  }

  Future<void> startConnection(String sessionId) async {
    if (_isConnected) return;

    _hubConnection =
        HubConnectionBuilder()
            .withUrl(
              '${ApiConstants.bookingServiceBaseUrl}/seatsHub',
              options: HttpConnectionOptions(
                accessTokenFactory: () async => _accessToken ?? "",
              ),
            )
            .withAutomaticReconnect()
            .build();

    _hubConnection.on('SeatChanged', (List<Object?>? arguments) {
      if (arguments != null && arguments.isNotEmpty) {
        _seatChangedController.add(arguments[0] as Map<String, dynamic>);
      }
    });

    try {
      await _hubConnection.start();
      _isConnected = true;
      await _hubConnection.invoke('JoinSession', args: [sessionId]);
    } catch (e) {
      _isConnected = false;
      throw Exception('Не удалось подключиться к SignalR: $e');
    }
  }

  Future<void> stopConnection(String sessionId) async {
    if (!_isConnected) return;

    try {
      await _hubConnection.invoke('LeaveSession', args: [sessionId]);
      await _hubConnection.stop();
      _isConnected = false;
    } catch (e) {
      // Игнорируем ошибки при отключении
    }
  }

  void dispose() {
    _seatChangedController.close();
  }
}
