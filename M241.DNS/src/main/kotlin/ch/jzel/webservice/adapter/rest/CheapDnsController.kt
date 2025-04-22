package ch.jzel.webservice.adapter.rest

import org.springframework.web.bind.annotation.GetMapping
import org.springframework.web.bind.annotation.RestController
import java.net.InetAddress.getLocalHost

@RestController
class CheapDnsController {
    @GetMapping
    fun getIP(): String {
        return getLocalHost().hostAddress
    }
}
